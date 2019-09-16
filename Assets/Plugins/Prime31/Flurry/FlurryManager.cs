using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;



#if UNITY_IOS || UNITY_TVOS

namespace Prime31
{
	public class FlurryManager : AbstractManager
	{
		/// <summary>
		/// Fired when an interstitial successfully loads
		/// </summary>
		public static event Action<string> interstitialLoadedEvent;
	
		/// <summary>
		/// Fired when an interstitial fails to load. The first parameter is the ad space and the second is the error message.
		/// </summary>
		public static event Action<string,string> interstitialFailedEvent;
	
		/// <summary>
		/// Fired when an interstitial is displayed
		/// </summary>
		public static event Action<string> interstitialDisplayedEvent;
	
		/// <summary>
		/// Fired when an interstitial is dismissed
		/// </summary>
		public static event Action<string> interstitialDismissedEvent;

		/// <summary>
		/// Fired when an interstitial video finishes playing
		/// </summary>
		public static event Action<string> interstitialVideoFinishedEvent;

		/// <summary>
		/// Fired when a banner ad loads successfully
		/// </summary>
		public static event Action<string> bannerLoadedEvent;
	
		/// <summary>
		/// Fired when a banner ad fails to load. The first parameter is the ad space and the second is the error message.
		/// </summary>
		public static event Action<string,string> bannerFailedEvent;

		/// <summary>
		/// Fired when a banner will present itself full screen
		/// </summary>
		public static event Action<string> bannerWillPresentFullScreenEvent;

		/// <summary>
		/// Fired when a banner dismisses itself from displaying full screen
		/// </summary>
		public static event Action<string> bannerWillDismissFullScreenEvent;
		
	
	
		static FlurryManager()
		{
			AbstractManager.initialize( typeof( FlurryManager ) );
		}
	
	
		void interstitialLoaded( string space )
		{
			if( interstitialLoadedEvent != null )
				interstitialLoadedEvent( space );
		}
	
	
		void interstitialFailed( string json )
		{
			if( interstitialFailedEvent != null )
			{
				var dict = Json.decode<Dictionary<string,string>>( json );
				interstitialFailedEvent( dict["space"], dict["error"] );
			}
		}
	
	
		void interstitialDisplayed( string space )
		{
			if( interstitialDisplayedEvent != null )
				interstitialDisplayedEvent( space );
		}
	
	
		void interstitialDismissed( string space )
		{
			if( interstitialDismissedEvent != null )
				interstitialDismissedEvent( space );
		}


		void interstitialVideoDidFinish( string space )
		{
			interstitialVideoFinishedEvent.fire( space );
		}
	
	
		void bannerLoaded( string space )
		{
			if( bannerLoadedEvent != null )
				bannerLoadedEvent( space );
		}
	
	
		void bannerFailed( string json )
		{
			if( bannerFailedEvent != null )
			{
				var dict = Json.decode<Dictionary<string,string>>( json );
				bannerFailedEvent( dict["space"], dict["error"] );
			}
		}


		void bannerWillPresentFullScreen( string space )
		{
			bannerWillPresentFullScreenEvent.fire( space );
		}


		void bannerWillDismissFullScreen( string space )
		{
			bannerWillDismissFullScreenEvent.fire( space );
		}
	
	}

}

#endif
