using SpecDrill;
using SpecDrill.Secondary.Ports.AutomationFramework;

namespace SomeTests.PageObjects.Popups
{
    public class PopupPage : WebPage
    {
        [Find(By.Id, "btnOpenPN")]
        [FindTarget(nameof(ThePopup))] // Throw if (ThePopup) is not a member or if ThePopup is not a field returning IElement and having Find Attribute applied
        public INavigationElement<Popup> BtnOpenPopupPN { get; private set; }

        [Find(By.Id, "btnOpenS")]
        [FindTarget(By.ClassName, "popup")]
        public INavigationElement<Popup> BtnOpenPopupS { get; private set; }

        [Find(By.Id, "btnOpenS")]
        public INavigationElement<Popup> BtnOpenPopupWC { get; private set; }

        [Find(By.ClassName, "popup")]
        public Popup ThePopup { get; private set; }
    }

    public class ErrPopupPageFindTargetPropertyIsNotINavigationTargetOrHasNoFindAttribute : WebPage
    {
        [Find(By.Id, "btnOpenPN")]
        [FindTarget(nameof(BtnOpenPopupS))] // Throw if (ThePopup) is not a member or if ThePopup is not a field returning INavigationTargetElement and having Find Attribute applied
        public INavigationElement<Popup> BtnOpenPopupPN { get; private set; }

        [Find(By.Id, "btnOpenS")]
        [FindTarget(By.ClassName, "popup")]
        public INavigationElement<Popup> BtnOpenPopupS { get; private set; }

        [Find(By.ClassName, "popup")]
        public IElement ThePopup { get; private set; }
    }

    public class ErrPopupNoTargetLocator : WebPage
    {
        [Find(By.Id, "btnOpenS")]
        public INavigationElement<PopupNoFindAttribute> BtnOpenPopupWrong { get; private set; } //Throw (FindTarget not defined!)

        [Find(By.ClassName, "popup")]
        public PopupNoFindAttribute ThePopup { get; private set; }
    }

    [Find(By.ClassName, "popup")]
    public class Popup : WebControl
    {
        public Popup(IElement? parent, IElementLocator elementLocator) : base(parent, elementLocator) { }
        
        [Find(By.Id, "txt")]
        public IElement Message { get; private set; }

        [Find(By.Id, "btnClose")]
        public IElement Close { get; private set; }
    }

    public class PopupNoFindAttribute : WebControl
    {
        public PopupNoFindAttribute(IElement? parent, IElementLocator elementLocator) : base(parent, elementLocator) { }

        [Find(By.Id, "txt")]
        public IElement Message { get; private set; }

        [Find(By.Id, "btnClose")]
        public IElement Close { get; private set; }
    }
}
