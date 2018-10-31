package md556c622d6e29a3ee7c3d6dcda3d952db1;


public class MyViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("CareVisit.Droid.MyViewHolder, CareVisit.Droid", MyViewHolder.class, __md_methods);
	}


	public MyViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == MyViewHolder.class)
			mono.android.TypeManager.Activate ("CareVisit.Droid.MyViewHolder, CareVisit.Droid", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
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
