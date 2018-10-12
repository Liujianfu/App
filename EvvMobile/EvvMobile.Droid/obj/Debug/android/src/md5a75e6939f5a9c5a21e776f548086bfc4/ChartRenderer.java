package md5a75e6939f5a9c5a21e776f548086bfc4;


public class ChartRenderer
	extends md51558244f76c53b6aeda52c8a337f2c37.ViewRenderer_2
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("EvvMobile.Droid.Charts.ChartRenderer, EvvMobile.Droid", ChartRenderer.class, __md_methods);
	}


	public ChartRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == ChartRenderer.class)
			mono.android.TypeManager.Activate ("EvvMobile.Droid.Charts.ChartRenderer, EvvMobile.Droid", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public ChartRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == ChartRenderer.class)
			mono.android.TypeManager.Activate ("EvvMobile.Droid.Charts.ChartRenderer, EvvMobile.Droid", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public ChartRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == ChartRenderer.class)
			mono.android.TypeManager.Activate ("EvvMobile.Droid.Charts.ChartRenderer, EvvMobile.Droid", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
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
