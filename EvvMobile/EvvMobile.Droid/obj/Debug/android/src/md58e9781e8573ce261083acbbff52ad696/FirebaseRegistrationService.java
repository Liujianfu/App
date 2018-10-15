package md58e9781e8573ce261083acbbff52ad696;


public class FirebaseRegistrationService
	extends com.google.firebase.iid.FirebaseInstanceIdService
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onTokenRefresh:()V:GetOnTokenRefreshHandler\n" +
			"";
		mono.android.Runtime.register ("EvvMobile.Droid.Notifications.FirebaseRegistrationService, EvvMobile.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", FirebaseRegistrationService.class, __md_methods);
	}


	public FirebaseRegistrationService ()
	{
		super ();
		if (getClass () == FirebaseRegistrationService.class)
			mono.android.TypeManager.Activate ("EvvMobile.Droid.Notifications.FirebaseRegistrationService, EvvMobile.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onTokenRefresh ()
	{
		n_onTokenRefresh ();
	}

	private native void n_onTokenRefresh ();

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
