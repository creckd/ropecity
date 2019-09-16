using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;



#if UNITY_ANDROID
namespace Prime31
{
	public enum FlurryGender
	{
		Male,
		Female,
		Unknown
	}


	public class FlurryAnalytics
	{
		// store the FlurryAgent class so we can make calls easily
		private static AndroidJavaClass _flurryAgent;
		private static AndroidJavaObject _plugin;


		static FlurryAnalytics()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_flurryAgent = new AndroidJavaClass( "com.flurry.android.FlurryAgent" );

			// find the plugin instance
			using( var pluginClass = new AndroidJavaClass( "com.prime31.analytics.FlurryAnalytics" ) )
				_plugin = pluginClass.CallStatic<AndroidJavaObject>( "instance" );
		}


		/// <summary>
		/// Starts up your Flurry session. Call on application startup. Passing true for enableLogging will enable verbose debug logs for Flurrys SDK
		/// </summary>
		public static void startSession( string apiKey, bool enableLogging = false )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "onStartSession", apiKey, enableLogging );
		}


		/// <summary>
		/// Ends your Flurry session. This is automatically called for you when your app is backgrounded so unless you have a good reason to call it you should never need to use it directly.
		/// </summary>
		public static void onEndSession()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "onEndSession" );
		}


		/// <summary>
		/// Changes the window during which a session can be resumed.  Must be called before onStartSession!
		/// </summary>
		public static void setContinueSessionMillis( long milliseconds )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_flurryAgent.CallStatic( "setContinueSessionMillis", milliseconds );
		}


		/// <summary>
		/// Logs an event to Flurry that is optionally timed
		/// </summary>
		public static void logEvent( string eventName )
		{
			logEvent( eventName, false );
		}


		public static void logEvent( string eventName, bool isTimed )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			if( isTimed )
				_plugin.Call( "logTimedEvent", eventName );
			else
				_plugin.Call( "logEvent", eventName );
		}


		/// <summary>
		/// Logs an event with optional key/value pairs that is optionally timed
		/// </summary>
		public static void logEvent( string eventName, Dictionary<string,string> parameters )
		{
			logEvent( eventName, parameters, false );
		}


		public static void logEvent( string eventName, Dictionary<string,string> parameters, bool isTimed )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			if( parameters == null )
			{
				Debug.LogError( "attempting to call logEvent with null parameters" );
				return;
			}

			if( isTimed )
				_plugin.Call( "logTimedEventWithParams", eventName, parameters.toJson() );
			else
				_plugin.Call( "logEventWithParams", eventName, parameters.toJson() );
		}


		/// <summary>
		/// Ends a timed event with optional key/value pairs
		/// </summary>
		public static void endTimedEvent( string eventName )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "endTimedEvent", eventName );
		}


		public static void endTimedEvent( string eventName, Dictionary<string,string> parameters )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			if( parameters == null )
			{
				Debug.LogError( "attempting to call endTimedEvent with null parameters" );
				return;
			}

			_plugin.Call( "endTimedEventWithParams", eventName, parameters.toJson() );
		}


		/// <summary>
		/// Logs a payment with all transaction details
		/// </summary>
		public static void logPayment( string productName, string productId, int quantity, double price, string currency, string transactionId, Dictionary<string,string> parameters = null )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			if( parameters == null )
				parameters = new Dictionary<string, string>();

			_plugin.Call( "logPayment", productName, productId, quantity, price, currency, transactionId, parameters.toJson() );
		}


		/// <summary>
		/// Use onPageView to report page view count
		/// </summary>
		public static void onPageView()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_flurryAgent.CallStatic( "onPageView" );
		}


		/// <summary>
		/// Use onError to report application errors
		/// </summary>
		public static void onError( string errorId, string message, string errorClass )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_flurryAgent.CallStatic( "onError", errorId, message, errorClass );
		}


		/// <summary>
		/// Use this to log the user's assigned ID or username in your system
		/// </summary>
		public static void setUserID( string userId )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_flurryAgent.CallStatic( "setUserId", userId );
		}


		/// <summary>
		/// Use this to log the user's age. Valid inputs are between 1 and 109.
		/// </summary>
		public static void setAge( int age )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_flurryAgent.CallStatic( "setAge", age );
		}


		/// <summary>
		/// Sends the user's gender to Flurrys servers.
		/// </summary>
		public static void setGender( FlurryGender gender )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			var genderObject = new AndroidJavaClass( "com.flurry.android.Contants" ).GetStatic<AndroidJavaObject>( gender.ToString().ToUpper() );
			_flurryAgent.CallStatic( "setGender", genderObject );
		}


		/// <summary>
		/// To enable/disable FlurryAgent logging call
		/// </summary>
		public static void setLogEnabled( bool enable )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_flurryAgent.CallStatic( "setLogEnabled", enable );
		}

	}
}
#endif
