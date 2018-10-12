package md5706c568089ed43df4cfd54d951f4f2d6;


public class TextDrawable
	extends android.graphics.drawable.ColorDrawable
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_draw:(Landroid/graphics/Canvas;)V:GetDraw_Landroid_graphics_Canvas_Handler\n" +
			"";
		mono.android.Runtime.register ("EvvMobile.Droid.Customizations.CustomControls.Calendar.TextDrawable, EvvMobile.Droid", TextDrawable.class, __md_methods);
	}


	public TextDrawable ()
	{
		super ();
		if (getClass () == TextDrawable.class)
			mono.android.TypeManager.Activate ("EvvMobile.Droid.Customizations.CustomControls.Calendar.TextDrawable, EvvMobile.Droid", "", this, new java.lang.Object[] {  });
	}


	public TextDrawable (int p0)
	{
		super (p0);
		if (getClass () == TextDrawable.class)
			mono.android.TypeManager.Activate ("EvvMobile.Droid.Customizations.CustomControls.Calendar.TextDrawable, EvvMobile.Droid", "Android.Graphics.Color, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public void draw (android.graphics.Canvas p0)
	{
		n_draw (p0);
	}

	private native void n_draw (android.graphics.Canvas p0);

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
