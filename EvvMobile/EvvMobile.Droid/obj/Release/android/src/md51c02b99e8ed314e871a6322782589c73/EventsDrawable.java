package md51c02b99e8ed314e871a6322782589c73;


public class EventsDrawable
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
		mono.android.Runtime.register ("EvvMobile.Droid.Customizations.CustomControls.Calendar.EventsDrawable, EvvMobile.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", EventsDrawable.class, __md_methods);
	}


	public EventsDrawable ()
	{
		super ();
		if (getClass () == EventsDrawable.class)
			mono.android.TypeManager.Activate ("EvvMobile.Droid.Customizations.CustomControls.Calendar.EventsDrawable, EvvMobile.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public EventsDrawable (int p0)
	{
		super (p0);
		if (getClass () == EventsDrawable.class)
			mono.android.TypeManager.Activate ("EvvMobile.Droid.Customizations.CustomControls.Calendar.EventsDrawable, EvvMobile.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Graphics.Color, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
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