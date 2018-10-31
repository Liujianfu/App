package md58619ac96477ea91c7dc8553bcd0781ee;


public class SlidingPaneActivity
	extends android.support.v7.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("CareVisit.Droid.Activities.SlidingPaneActivity, CareVisit.Droid", SlidingPaneActivity.class, __md_methods);
	}


	public SlidingPaneActivity ()
	{
		super ();
		if (getClass () == SlidingPaneActivity.class)
			mono.android.TypeManager.Activate ("CareVisit.Droid.Activities.SlidingPaneActivity, CareVisit.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
