namespace mFramework.UI
{
    public abstract class UIComponent : UIObject
    {
        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
        }
    }
}