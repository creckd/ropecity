using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;



namespace Prime31
{
	public class FlurryAndroidManager : AbstractManager
	{
		/// <summary>
		/// Fired when an ad is successfully fetched from the server
		/// </summary>
		public static event Action<string> onFetchedEvent;

		/// <summary>
		/// Fired when an ad is rendered
		/// </summary>
		public static event Action<string> onRenderedEvent;

		/// <summary>
		/// Interstitial ads only. Fired when an ad is displayed.
		/// </summary>
		public static event Action<string> onDisplayEvent;

		/// <summary>
		/// Interstitial ads only. Fired when an ad is closed.
		/// </summary>
		public static event Action<string> onCloseEvent;

		/// <summary>
		/// Fired when an ad causes a new application to be opend
		/// </summary>
		public static event Action<string> onAppExitEvent;

		/// <summary>
		/// Fired when an ad is clicked
		/// </summary>
		public static event Action<string> onClickedEvent;

		/// <summary>
		/// Fired when a video ad completes
		/// </summary>
		public static event Action<string> onVideoCompletedEvent;

		/// <summary>
		/// Fired when a render error occurs
		/// </summary>
		public static event Action<string> onRenderErrorEvent;

		/// <summary>
		/// Fired when an ad fails to download
		/// </summary>
		public static event Action<string> onFetchErrorEvent;

		/// <summary>
		/// Banner ads only. Fired when a banner takes over the screen.
		/// </summary>
		public static event Action<string> onShowFullscreenEvent;

		/// <summary>
		/// Banner ads only. Fired when a banner dismisses its full screen control.
		/// </summary>
		public static event Action<string> onCloseFullscreenEvent;


		static FlurryAndroidManager()
		{
			AbstractManager.initialize( typeof( FlurryAndroidManager ) );
		}


		void onFetched( string adSpace )
		{
			onFetchedEvent.fire( adSpace );
		}


		void onRendered( string adSpace )
		{
			onRenderedEvent.fire( adSpace );
		}


		void onDisplay( string adSpace )
		{
			onDisplayEvent.fire( adSpace );
		}


		void onClose( string adSpace )
		{
			onCloseEvent.fire( adSpace );
		}


		void onAppExit( string adSpace )
		{
			onAppExitEvent.fire( adSpace );
		}


		void onClicked( string adSpace )
		{
			onClickedEvent.fire( adSpace );
		}


		void onVideoCompleted( string adSpace )
		{
			onVideoCompletedEvent.fire( adSpace );
		}


		void onRenderError( string adSpace )
		{
			onRenderErrorEvent.fire( adSpace );
		}


		void onFetchError( string adSpace )
		{
			onFetchErrorEvent.fire( adSpace );
		}


		void onShowFullscreen( string adSpace )
		{
			onShowFullscreenEvent.fire( adSpace );
		}


		void onCloseFullscreen( string adSpace )
		{
			onCloseFullscreenEvent.fire( adSpace );
		}

	}

}

