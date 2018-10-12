package md5c302972cdd8a8c9a35d3baece23bdd92;


public class ImageButtonRenderer
	extends md51558244f76c53b6aeda52c8a337f2c37.ButtonRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("EvvMobile.Droid.Customizations.CustomControls.ImageButtonRenderer, EvvMobile.Droid", ImageButtonRenderer.class, __md_methods);
	}


	public ImageButtonRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == ImageButtonRenderer.class)
			mono.android.TypeManager.Activate ("EvvMobile.Droid.Customizations.CustomControls.ImageButtonRenderer, EvvMobile.Droid", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public ImageButtonRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == ImageButtonRenderer.class)
			mono.android.TypeManager.Activate ("EvvMobile.Droid.Customizations.CustomControls.ImageButtonRenderer, EvvMobile.Droid", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public ImageButtonRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == ImageButtonRenderer.class)
			mono.android.TypeManager.Activate ("EvvMobile.Droid.Customizations.CustomControls.ImageButtonRenderer, EvvMobile.Droid", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
