//
//  VungleBinding.m
//  VungleTest
//
//
#import <VungleSDK/VungleSDK.h>
#import "VungleManager.h"


// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

// Converts C style string to NSString as long as it isnt empty
#define GetStringParamOrNil( _x_ ) ( _x_ != NULL && strlen( _x_ ) ) ? [NSString stringWithUTF8String:_x_] : nil

#define VUNGLE_API_KEY   @"vungle.api_endpoint"

void UnitySendMessage( const char * className, const char * methodName, const char * param );
UIViewController *UnityGetGLViewController();

static BOOL bInit = false;
void _vungleStartWithAppId( const char * appId, const char* placements[], int placementsCount, const char * pluginVersion )
{
	if (bInit)
		return;
	if( [[VungleSDK sharedSDK] respondsToSelector:@selector(setPluginName:version:)] )
		[[VungleSDK sharedSDK] performSelector:@selector(setPluginName:version:) withObject:@"unity" withObject:GetStringParam(pluginVersion)];

    NSError * error;
    NSMutableArray * _placements = [NSMutableArray array];
    for (int i = 0; i < placementsCount; i++)
        [_placements addObject:GetStringParam(placements[i])];
    
    [VungleSDK sharedSDK].delegate = [VungleManager sharedManager];
    [[VungleSDK sharedSDK] startWithAppId:GetStringParam(appId) placements:_placements error:&error];
    bInit = true;
    
    [[VungleSDK sharedSDK] setLoggingEnabled:true];
    [[VungleSDK sharedSDK] attachLogger:[VungleManager sharedManager]];
}


void _vungleSetSoundEnabled( BOOL enabled )
{
	[VungleSDK sharedSDK].muted = !enabled;
}


void _vungleEnableLogging( BOOL shouldEnable )
{
	[[VungleSDK sharedSDK] setLoggingEnabled:shouldEnable];
}


BOOL _vungleIsAdAvailable(const char* placementID)
{
	return [[VungleSDK sharedSDK] isAdCachedForPlacementID:GetStringParam(placementID)];
}

UIInterfaceOrientationMask makeOrientation(NSNumber* code) {
    UIInterfaceOrientationMask orientationMask;
    int i = [code intValue];
    switch( i )
    {
        case 1:
            orientationMask = UIInterfaceOrientationMaskPortrait;
            break;
        case 2:
            orientationMask = UIInterfaceOrientationMaskLandscapeLeft;
            break;
        case 3:
            orientationMask = UIInterfaceOrientationMaskLandscapeRight;
            break;
        case 4:
            orientationMask = UIInterfaceOrientationMaskPortraitUpsideDown;
            break;
        case 5:
            orientationMask = UIInterfaceOrientationMaskLandscape;
            break;
        case 6:
            orientationMask = UIInterfaceOrientationMaskAll;
            break;
        case 7:
            orientationMask = UIInterfaceOrientationMaskAllButUpsideDown;
            break;
        default:
            orientationMask = UIInterfaceOrientationMaskAllButUpsideDown;
    }
    return orientationMask;
}

BOOL _vungleLoadAd(const char* placementID)
{
    NSError * error;
    return [[VungleSDK sharedSDK] loadPlacementWithID:GetStringParam(placementID) error:&error];
}

void _vunglePlayAd(char* opt, const char* placementID) {
    NSObject* obj = [VungleManager objectFromJson:GetStringParam( opt )];
    if([obj isKindOfClass:[NSDictionary class]])
    {
        NSError * error;
        NSDictionary *from = obj;
        NSMutableDictionary *options = [NSMutableDictionary dictionary];
//        options[VunglePlayAdOptionKeyIncentivized] = from[@"incentivized"];
        options[VunglePlayAdOptionKeyOrientations] = @(makeOrientation(from[@"orientation"]));
//        options[VunglePlayAdOptionKeyLargeButtons] = from[@"large"];
        if (from[@"userTag"])
            options[VunglePlayAdOptionKeyUser]  = from[@"userTag"];
        if (from[@"alertTitle"])
            options[VunglePlayAdOptionKeyIncentivizedAlertTitleText] = from[@"alertTitle"];
        if (from[@"alertText"])
            options[VunglePlayAdOptionKeyIncentivizedAlertBodyText] = from[@"alertText"];
        if (from[@"closeText"])
            options[VunglePlayAdOptionKeyIncentivizedAlertCloseButtonText] = from[@"closeText"];
        if (from[@"continueText"])
            options[VunglePlayAdOptionKeyIncentivizedAlertContinueButtonText] = from[@"continueText"];
        //options[VunglePlayAdOptionKeyPlacement] = from[@"placement"];
        /*
        NSMutableDictionary *extra = [NSMutableDictionary dictionary];
        if (from[@"key1"])
            extra[VunglePlayAdOptionKeyExtra1] = from[@"key1"];
        if (from[@"key2"])
            extra[VunglePlayAdOptionKeyExtra2] = from[@"key2"];
        if (from[@"key3"])
            extra[VunglePlayAdOptionKeyExtra3] = from[@"key3"];
        if (from[@"key4"])
            extra[VunglePlayAdOptionKeyExtra4] = from[@"key4"];
        if (from[@"key5"])
            extra[VunglePlayAdOptionKeyExtra5] = from[@"key5"];
        if (from[@"key6"])
            extra[VunglePlayAdOptionKeyExtra6] = from[@"key6"];
        if (from[@"key7"])
            extra[VunglePlayAdOptionKeyExtra7] = from[@"key7"];
        if (from[@"key8"])
            extra[VunglePlayAdOptionKeyExtra8] = from[@"key8"];
        options[VunglePlayAdOptionKeyExtraInfoDictionary] = extra;
         */
        [[VungleSDK sharedSDK] playAd:UnityGetGLViewController() options:options placementID:GetStringParam(placementID) error:&error];
    }
}

void _vungleClearSleep( )
{
    [[VungleSDK sharedSDK] clearSleep];
}

void _vungleSetEndPoint(const char * endPoint) {
    NSString *endPointString = GetStringParamOrNil( endPoint );
    if (endPointString) {
        NSUserDefaults* defaults = [NSUserDefaults standardUserDefaults];
        [defaults setObject:endPointString forKey:VUNGLE_API_KEY];
    }
}

static char* MakeStringCopy (const char* string) {
    if (string == NULL) {
        return NULL;
    }
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

char * _vungleGetEndPoint() {
    NSUserDefaults* defaults = [NSUserDefaults standardUserDefaults];
    return MakeStringCopy([[defaults objectForKey:VUNGLE_API_KEY] UTF8String]);
}


