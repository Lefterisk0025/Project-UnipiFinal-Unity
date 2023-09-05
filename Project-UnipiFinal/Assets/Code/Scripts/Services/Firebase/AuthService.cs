using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System.Threading;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using TMPro;
using System.Threading.Tasks;

public class AuthService
{
    string authCode;

    public bool AuthGooglePlayGames()
    {
        try
        {
            bool errorFound = false;
            PlayGamesPlatform.Instance.Authenticate(status =>
            {
                if (status == SignInStatus.Success)
                {
                    PlayGamesPlatform.Instance.RequestServerSideAccess(true, async code =>
                   {
                       authCode = code;
                       Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                       Firebase.Auth.Credential credential = Firebase.Auth.PlayGamesAuthProvider.GetCredential(authCode);
                       await auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                       {
                           if (task.IsCanceled)
                           {
                               // Handle task canceled
                               errorFound = true;
                           }
                           else if (task.IsFaulted)
                           {
                               // Handle task faulted
                               errorFound = true;
                           }
                           else
                           {
                               Firebase.Auth.FirebaseUser user = task.Result;

                               Debug.LogFormat("User signed in successfully: {0} ({1})",
                                   user.DisplayName, user.UserId);
                           }
                       });
                   });
                }
                else
                {
                    errorFound = true;
                }
            });

            if (errorFound)
                return false;
            else
                return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }

    public bool AuthAnonymously()
    {
        try
        {
            bool errorFound = false;
            Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    errorFound = true;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    errorFound = true;
                }

                Firebase.Auth.AuthResult result = task.Result;

                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);
            });

            if (errorFound)
                return false;
            else
                return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }

    public void ConvertAnonymousAccountToPermanent()
    {
        try
        {
            // Auth with play games
            PlayGamesPlatform.Instance.Authenticate(status =>
            {
                if (status == SignInStatus.Success)
                {
                    PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                    {
                        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                        Firebase.Auth.Credential credential = Firebase.Auth.PlayGamesAuthProvider.GetCredential(code);
                        // After success auth, link accounts
                        auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task =>
                        {
                            if (task.IsCanceled)
                            {
                                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                                return;
                            }
                            else if (task.IsFaulted)
                            {
                                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                                return;
                            }
                            else
                            {
                                Firebase.Auth.AuthResult result = task.Result;

                                Debug.LogFormat("User signed in successfully: {0} ({1})",
                                    result.User.DisplayName, result.User.UserId);

                                string newDisplayName = PlayGamesPlatform.Instance.GetUserDisplayName();
                                UpdateCurrentUserDisplayName(newDisplayName);
                            }
                        });
                    });
                }
            });
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private bool UpdateCurrentUserDisplayName(string displayName)
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            // Create a new user profile
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = displayName,
            };

            // Update user's profile
            user.UpdateUserProfileAsync(profile).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");
            });
        }

        return false;
    }
}

