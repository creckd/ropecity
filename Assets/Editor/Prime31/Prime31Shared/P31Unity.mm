//
//  P31Unity.m
//  P31SharedTools
//
//  Copyright (c) 2018 prime31. All rights reserved.
//

#import "P31Unity.h"



#ifdef __cplusplus
extern "C" {
#endif
	void UnitySendMessage( const char * className, const char * methodName, const char * param );
#ifdef __cplusplus
}
#endif


void UnityPause( int pause );


@implementation P31Unity

+ (void)unityPause:(NSNumber*)shouldPause
{
	UnityPause( shouldPause.intValue );
}

@end
