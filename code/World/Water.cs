public sealed class Water : Component
{
	protected override void OnPreRender()
	{
		var sceneObject = GameObject.Components.Get<ModelRenderer>().SceneObject;
		sceneObject.Flags.IsTranslucent = true;
		sceneObject.Flags.IsOpaque = false;
	}
}
