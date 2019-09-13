using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Prime31;




#if UNITY_IOS || UNITY_TVOS
namespace Prime31
{
	public class FlurryAnalytics
	{
		[DllImport("__Internal")]
		private static extern void _flurryStartSession( string apiKey, bool enableCrashReporting );

		/// <summary>
		/// Starts up your Flurry analytics session. Call at application startup and you can optionally enable crash reporting here.
		/// </summary>
		/// <param name="apiKey">API key.</param>
		/// <param name="enableCrashReporting">If set to <c>true</c> enable crash reporting.</param>
		public static void startSession( string apiKey, bool enableCrashReporting = false )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS )
				_flurryStartSession( apiKey, enableCrashReporting );
		}


		[DllImport("__Internal")]
		private static extern void _flurrySetDebugLogEnabled( bool shouldEnable );

		/// <summary>
		/// Enables or disables Flurry SDK debug logs
		/// </summary>
		/// <param name="shouldEnable">If set to <c>true</c> should enable.</param>
		public static void setDebugLogEnabled( bool shouldEnable )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS )
				_flurrySetDebugLogEnabled( shouldEnable );
		}


		[DllImport("__Internal")]
		private static extern void _flurrySetAppVersion( string appVersion );

		/// <summary>
		/// Sets the app version for this build overriding the CFBundleVersion
		/// </summary>
		/// <param name="appVersion">App version.</param>
		public static void setAppVersion( string appVersion )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS )
				_flurrySetAppVersion( appVersion );
		}


		[DllImport("__Internal")]
		private static extern void _flurryLogEvent( string eventName, bool isTimed );

		// Logs an event to Flurry. If isTimed is true, this will be a timed event and it should be paired with a call to endTimedEvent
		public static void logEvent( string eventName, bool isTimed )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS )
				_flurryLogEvent( eventName, isTimed );
		}
		
		
	    /// <summary>
		/// Logs an event with optional key/value pairs that is optionally timed
	    /// </summary>
	    /// <param name="eventName">Event name.</param>
	    /// <param name="parameters">Parameters.</param>
	    public static void logEvent( string eventName, Dictionary<string,string> parameters )
	    {
	        logEventWithParameters( eventName, parameters, false );
	    }


		[DllImport("__Internal")]
		private static extern void _flurryLogEventWithParameters( string eventName, string parameters, bool isTimed );

		/// <summary>
		/// Logs an event with optional key/value pairs
		/// </summary>
		/// <param name="eventName">Event name.</param>
		/// <param name="parameters">Parameters.</param>
		/// <param name="isTimed">If set to <c>true</c> is timed.</param>
		public static void logEventWithParameters( string eventName, Dictionary<string,string> parameters, bool isTimed )
		{
			if( parameters == null )
				parameters = new Dictionary<string, string>();

			if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS )
				_flurryLogEventWithParameters( eventName, parameters.toJson(), isTimed );
		}


		[DllImport("__Internal")]
		private static extern void _flurryEndTimedEvent( string eventName, string parameters );

		/// <summary>
		/// Ends a timed event that was previously started
		/// </summary>
		/// <param name="eventName">Event name.</param>
		public static void endTimedEvent( string eventName )
		{
			endTimedEvent( eventName, new Dictionary<string,string>() );
		}

		public static void endTimedEvent( string eventName, Dictionary<string,string> parameters )
		{
			if( parameters == null )
				parameters = new Dictionary<string, string>();

			if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS )
				_flurryEndTimedEvent( eventName, parameters.toJson() );
		}


		[DllImport("__Internal")]
		private static extern void _flurrySetAge( int age );

		/// <summary>
		/// Sets the users age
		/// </summary>
		/// <param name="age">Age.</param>
		public static void setAge( int age )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS )
				_flurrySetAge( age );
		}


		[DllImport("__Internal")]
		private static extern void _flurrySetGender( string gender );

		// Sets the users gender
		public static void setGender( string gender )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS )
				_flurrySetGender( gender );
		}


		[DllImport("__Internal")]
		private static extern void _flurrySetUserId( string userId );

		/// <summary>
		/// Sets the users unique id
		/// </summary>
		/// <param name="userId">User identifier.</param>
		public static void setUserId( string userId )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS )
				_flurrySetUserId( userId );
		}


		[DllImport("__Internal")]
		private static extern void _flurrySetSessionReportsOnCloseEnabled( bool sendSessionReportsOnClose );

		/// <summary>
		/// Sets whether Flurry should upload session reports when your app closes
		/// </summary>
		/// <param name="sendSessionReportsOnClose">If set to <c>true</c> send session reports on close.</param>
		public static void setSessionReportsOnCloseEnabled( bool sendSessionReportsOnClose )

		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS )
				_flurrySetSessionReportsOnCloseEnabled( sendSessionReportsOnClose );
		}


		[DllImport("__Internal")]
		private static extern void _flurrySetSessionReportsOnPauseEnabled( bool setSessionReportsOnPauseEnabled );

		/// <summary>
		/// Sets whether Flurry should upload session reports when your app is paused
		/// </summary>
		/// <param name="setSessionReportsOnPauseEnabled">If set to <c>true</c> set session reports on pause enabled.</param>
		public static void setSessionReportsOnPauseEnabled( bool setSessionReportsOnPauseEnabled )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS )
				_flurrySetSessionReportsOnPauseEnabled( setSessionReportsOnPauseEnabled );
		}
	}

}
#endif
