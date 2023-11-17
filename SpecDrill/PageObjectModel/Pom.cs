using SpecDrill.Secondary.Ports.AutomationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace SpecDrill.PageObjectModel
{
    public enum ElementTypes
    {
        Generic,
        TextInput,
        DropDown,
    } //TODO: put ElementTypes field in all objects

    // locator
    public record PomLocator(string type, string value);
    // atoms
    public record PomElement(string type, string name, PomLocator locator, List<string> tags, string? target, string? itemType);
    public record PomNavigationElement(string name, PomLocator locator, string target, List<string> tags) : PomElement("navigation_element", name, locator, tags, target, null);
    public record PomFrameElement(string name, PomLocator locator, string target, List<string> tags) : PomElement("frame_element", name, locator, tags, target, null);
    public record PomSelectElement(string name, PomLocator locator, List<string> tags) : PomElement("select_element", name, locator, tags, null, null);
    //public record PomSelectElement
    // components
    public record PomComponent(string name, List<PomElement> elements, List<string> tags);

    // component ref
    public record PomComponentRef(string type, string name, PomLocator locator, List<string> tags) : PomElement(type, name, locator, tags, null, null);
    public record PomComponentList(string name, PomLocator locator, List<string> tags, [NotNull] string itemType) : PomElement("list", name, locator, tags, null, itemType);
    public record PomPage(string name, List<string> tags, List<PomElement> elements) : PomComponent(name, elements, tags);
    public record PomSitemap(string name, string version, List<PomComponent> components, List<PomPage> pages);

    public static class WebPageExtensions
    {
        public static T? Property<T>(this WebPage @this, Type pageType, string propertyName) where T : class, IElement => ((object)@this).Property<T>(pageType, propertyName);
        public static T? Property<T>(this WebControl @this, Type pageType, string propertyName) where T : class, IElement => ((object)@this).Property<T>(pageType, propertyName);
        public static T? Property<T>(this object @this, Type pageType, string propertyName)
            where T : class, IElement
            => @this.Property(pageType, propertyName) as T;
        public static object Property(this object @this, Type pageType, string propertyName)
            => pageType.GetProperty(propertyName)?.GetValue(@this) ?? throw new Exception($"Property {propertyName} not found on {pageType} type");
    }

    public static class PomExtensions
    {
        internal static Lazy<ModuleBuilder> SpecDrillDynamicModule = new(() =>
        {
            var execAsm = Assembly.GetExecutingAssembly();
            AssemblyName dynAsmName = new("SpecDrillDynamicAssembly");
            dynAsmName.Name = execAsm.GetName().Name;

            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(dynAsmName, AssemblyBuilderAccess.Run);
            return asmBuilder.DefineDynamicModule("SpecDrillDynamicModule");
        });

        public static void AddProperty(this TypeBuilder tb, (Type Type, string Name) propertySignature, (By Type, string Value) selector)
        {
            FieldBuilder fb = tb.DefineField(
                $"m_{propertySignature.Name.ToLower()}",
                propertySignature.Type,
                FieldAttributes.Private);
            var pb = tb.DefineProperty(propertySignature.Name, PropertyAttributes.HasDefault, propertySignature.Type, null);
            MethodAttributes getSetAttr = MethodAttributes.Public |
                MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            // get accessor
            MethodBuilder mbPropertyAccessor = tb.DefineMethod(
               $"get_{propertySignature.Name}",
               getSetAttr,
               propertySignature.Type,
               Type.EmptyTypes);
            var propAccessorIL = mbPropertyAccessor.GetILGenerator();
            // For an instance property, argument zero is the instance. Load the
            // instance, then load the private field and return, leaving the
            // field value on the stack.
            propAccessorIL.Emit(OpCodes.Ldarg_0);
            propAccessorIL.Emit(OpCodes.Ldfld, fb);
            propAccessorIL.Emit(OpCodes.Ret);

            MethodBuilder mbPropertySetter = tb.DefineMethod(
                $"set_{propertySignature.Name}",
                getSetAttr,
                null,
                new Type[] { propertySignature.Type });

            ILGenerator propSetterIL = mbPropertySetter.GetILGenerator();
            // Load the instance and then the numeric argument, then store the
            // argument in the field.
            propSetterIL.Emit(OpCodes.Ldarg_0);
            propSetterIL.Emit(OpCodes.Ldarg_1);
            propSetterIL.Emit(OpCodes.Stfld, fb);
            propSetterIL.Emit(OpCodes.Ret);

            // Last, map the "get" and "set" accessor methods to the
            // PropertyBuilder. The property is now complete.
            pb.SetGetMethod(mbPropertyAccessor);
            pb.SetSetMethod(mbPropertySetter);

            Type findAttrType = typeof(FindAttribute);
            ConstructorInfo? findAttrInfo = findAttrType.GetConstructor(new Type[] { typeof(By), typeof(string) });

            if (findAttrInfo == null)
                throw new NullReferenceException($"ConstructorInfo? {nameof(findAttrInfo)} : should not be null!");

            var ab = new CustomAttributeBuilder(findAttrInfo, new[] { (object)selector.Type, selector.Value });

            pb.SetCustomAttribute(ab);
        }

        public static PomSitemap AddComponents(this PomSitemap @this, params PomComponent[] components)
        {
            foreach (var component in components)
            {
                @this.components.Add(component);
            }
            return @this;
        }
        public static PomSitemap AddPages(this PomSitemap @this, params PomPage[] pages)
        {
            foreach (var page in pages)
            {
                @this.pages.Add(page);
            }
            return @this;
        }
        public static PomElement Specialize(this PomElement element)
            => (element.type, element.itemType, element.target) switch
            {
                ("navigation_element", null, { }) => new PomNavigationElement(element.name, element.locator, element.target, element.tags),
                ("navigation_element", null, null) => throw new Exception($"Expected element.{nameof(element.target)} parameter is null!"),
                ("list", { }, null) => new PomComponentList(element.name, element.locator, element.tags, element.itemType),
                ("list", null, null) => throw new Exception($"Expected element.{nameof(element.itemType)} parameter is null!"),
                ("select_element", null, null) => new PomSelectElement(element.name, element.locator, element.tags),
                ("frame_element", null, { }) => new PomFrameElement(element.name, element.locator, element.target, element.tags),
                ("frame_element", null, null) => throw new Exception($"Expected element.{nameof(element.target)} parameter is null!"),
                ("element", null, null) => element,
                ({ }, null, null) => new PomComponentRef(element.type, element.name, element.locator, element.tags),
                (null, null, null) => throw new Exception($"Expected element.{nameof(element.type)} parameter is null!"),
                (_, _, _) => throw new Exception($"Unexpected combination: element.{nameof(element.type)} = {element.type}, element.{nameof(element.itemType)} = {element.itemType}, element.{nameof(element.target)} = {element.target} !"),
            };
        public static PomComponent Component(string name, List<string>? tags = default, params PomElement[] elements) => new(name, elements.ToList(), tags ?? new List<string>());
        public static PomComponentRef ComponentRef(string name, PomLocator locator, string type, List<string>? tags = default) => new(type, name, locator, tags ?? new List<string>());
        public static PomPage Page(string name, List<string>? tags = default, params PomElement[] elements) => new(name, tags ?? new List<string>(), elements.ToList());
        public static PomElement Element(string type, string name, PomLocator locator, string? target = null, string? itemType = null, List<string>? tags = default) => new(type, name, locator, tags ?? new List<string>(), target, itemType);
        public static PomNavigationElement NavigationElement(string name, PomLocator locator, string target, List<string>? tags = default) => new(name, locator, target, tags ?? new List<string>());
        public static PomFrameElement FrameElement(string name, PomLocator locator, string target, List<string>? tags = default) => new(name, locator, target, tags ?? new List<string>());
        public static PomSelectElement SelectElement(string name, PomLocator locator, List<string>? tags = default) => new(name, locator, tags ?? new());
        public static PomComponentList ComponentListElement(string name, PomLocator locator, string itemType, List<string>? tags = default)
        {
            if (string.IsNullOrWhiteSpace(itemType))
                throw new Exception($"Missing PomComponent : {itemType}. Component must be pre-decalred in order to be able to reference it. Is this a cyclic dependency?");

            return new(name, locator, tags ?? new List<string>(),
                itemType: itemType);
        }

        public static void AddElements(this PomComponent @this, params PomElement[] elements) => @this.elements.AddRange(elements);

        public static PomSitemap SiteMap(string name, string version, List<PomComponent> components, List<PomPage> pages)
            => new(name, version, components, pages);

        public static PomSitemap Build(this PomSitemap @this)
        {
            ModuleBuilder mb = SpecDrillDynamicModule.Value;

            var baseType = typeof(WebControl);
            var baseConstructor = baseType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { typeof(IElement), typeof(IElementLocator) })!; //we are sure we have that constructor in WebControl class!
            if (baseConstructor is null)
                throw new Exception("Expected (IElement, IElementLocator) constructor on WebControl type!");
            var components = @this.components.Concat(@this.pages);
            Dictionary<string, TypeBuilder> typeBuilders = new();
            foreach (var component in components)
            {
                var typeName = $"{@this.name}.{component.name}";
                if (mb.GetType(typeName) is { })
                    throw new Exception($"This Page Object component `{typeName}` already exists! Please use a different sitemap name / version!");

                switch (component)
                {
                    case PomComponent page when page is PomPage:
                        typeBuilders[typeName] = mb.DefineType(typeName, TypeAttributes.Public, typeof(WebPage));
                        break;
                    case PomComponent c when c is not PomPage:
                        var componentTb = mb.DefineType(typeName, TypeAttributes.Public, typeof(WebControl));
                        var cb_default = componentTb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Array.Empty<Type>());
                        var defaultCtorBody = cb_default.GetILGenerator();
                        defaultCtorBody.Emit(OpCodes.Ldarg_0);
                        defaultCtorBody.Emit(OpCodes.Ret);

                        var cb_2p = componentTb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(IElement), typeof(IElementLocator) });
                        var ctorBody = cb_2p.GetILGenerator();
                        ctorBody.Emit(OpCodes.Ldarg_0);
                        ctorBody.Emit(OpCodes.Ldarg_1);
                        ctorBody.Emit(OpCodes.Ldarg_2);
                        ctorBody.Emit(OpCodes.Call, baseConstructor);
                        ctorBody.Emit(OpCodes.Ret);
                        typeBuilders[typeName] = componentTb;

                        break;
                    default: throw new Exception($"unsupported component type {component.GetType().Name}");
                }
            }
            foreach (var component in components)
            {
                var typeName = $"{@this.name}.{component.name}";
                switch (component)
                {
                    case PomComponent page when page is PomPage:
                        var pageTb = typeBuilders[typeName];
                        pageTb.AddPropertiesToType(@this, page.elements);
                        pageTb.CreateType();
                        break;
                    case PomComponent c when c is not PomPage:
                        var componentTb = typeBuilders[typeName];

                        componentTb.AddPropertiesToType(@this, component.elements);
                        componentTb.CreateType();
                        break;
                    default: throw new Exception($"unsupported component type {component.GetType().Name}");
                }


            }

            return @this;
        }

        //public static PomSitemap BuildComponents(this PomSitemap @this)
        //{
        //    ModuleBuilder mb = SpecDrillDynamicModule.Value;

        //    var baseType = typeof(WebControl);
        //    var baseConstructor = baseType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { typeof(IElement), typeof(IElementLocator) })!; //we are sure we have that constructor in WebControl class!
        //    if (baseConstructor is null)
        //        throw new Exception("Expected (IElement, IElementLocator) constructor on WebControl type!");

        //    foreach (var component in @this.components)
        //    {
        //        var componentTb = mb.DefineType($"{@this.name}.{component.name}", TypeAttributes.Public, typeof(WebControl));
        //        var cb_default = componentTb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Array.Empty<Type>());
        //        var defaultCtorBody = cb_default.GetILGenerator();
        //        defaultCtorBody.Emit(OpCodes.Ldarg_0);
        //        defaultCtorBody.Emit(OpCodes.Ret);

        //        var cb_2p = componentTb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(IElement), typeof(IElementLocator) });
        //        var ctorBody = cb_2p.GetILGenerator();
        //        ctorBody.Emit(OpCodes.Ldarg_0);
        //        ctorBody.Emit(OpCodes.Ldarg_1);
        //        ctorBody.Emit(OpCodes.Ldarg_2);
        //        ctorBody.Emit(OpCodes.Call, baseConstructor);
        //        ctorBody.Emit(OpCodes.Ret);

        //        componentTb.AddPropertiesToType(@this, component.elements);
        //        componentTb.CreateType();
        //    }

        //    return @this;
        //}

        //public static PomSitemap BuildPages(this PomSitemap @this)
        //{
        //    ModuleBuilder mb = SpecDrillDynamicModule.Value;

        //    foreach (var page in @this.pages)
        //    {
        //        var pageTb = mb.DefineType($"{@this.name}.{page.name}", TypeAttributes.Public, typeof(WebPage));
        //        pageTb.AddPropertiesToType(@this, page.elements);
        //        pageTb.CreateType();
        //    }
        //    return @this;
        //}

        private static void AddPropertiesToType(this TypeBuilder @this, PomSitemap sitemap, List<PomElement> elements)
        {
            foreach (var element in elements)
            {
                var elementType = element.Specialize() switch
                {
                    PomNavigationElement pne => typeof(INavigationElement<>).MakeGenericType(sitemap.GetTypeOf(pne.target!)),
                    PomComponentList pcl => typeof(ListElement<>).MakeGenericType(sitemap.GetTypeOf(pcl.itemType!)),
                    PomComponentRef pcr => sitemap.GetTypeOf(pcr.type),
                    PomSelectElement pse => typeof(SelectElement),
                    PomFrameElement pfe => typeof(IFrameElement<>).MakeGenericType(sitemap.GetTypeOf(pfe.target!)),
                    PomElement e => typeof(IElement),
                    _ => null
                };

                if (elementType is null)
                    continue;

                AddProperty(@this, (elementType, element.name), (LocatorTypeFromString(element.locator.type), element.locator.value));
            }
        }

        private static By LocatorTypeFromString(string type)
       => type.ToLowerInvariant() switch
       {
           "id" => By.Id,
           "classname" => By.ClassName,
           "cssselector" => By.CssSelector,
           "xpath" => By.XPath,
           "name" => By.Name,
           "tagname" => By.TagName,
           "linktext" => By.LinkText,
           "partiallinktext" => By.PartialLinkText,
           _ => throw new Exception($"Unrecognised locator type `{type}`!")
       };

        public static Type GetTypeOf(this PomSitemap @this, string pageName) => SpecDrillDynamicModule.Value.GetType($"{@this.name}.{pageName}") ?? throw new Exception($"Page/Component {pageName} is not (yet) defined in dynamic assembly!");

    }
    internal class CDepsComparer : IComparer<(PomComponent c, string[] deps)>
    {
        public int Compare((PomComponent c, string[] deps) x, (PomComponent c, string[] deps) y)
        {
            var result = y.deps.Length - x.deps.Length;
            return result == 0 ?
                1 :
                result;
        }
    }
}
