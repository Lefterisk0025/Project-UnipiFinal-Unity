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

public class AuthService : MonoBehaviour
{
    string authCode;

    [SerializeField] private TextMeshProUGUI _usernameText;
    [SerializeField] private TextMeshProUGUI _loginStateText;

    public void LoginGooglePlayGames()
    {
        try
        {
            PlayGamesPlatform.Instance.Authenticate(status =>
            {
                if (status == SignInStatus.Success)
                {
                    PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                    {
                        authCode = code;
                        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                        Firebase.Auth.Credential credential = Firebase.Auth.PlayGamesAuthProvider.GetCredential(authCode);
                        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                        {
                            if (task.IsCanceled)
                            {
                                _loginStateText.text = "Login canceled";
                            }
                            else if (task.IsFaulted)
                            {
                                _loginStateText.text = "Login faulted";
                            }
                            else
                            {
                                _loginStateText.text = "Logged in";
                                Firebase.Auth.FirebaseUser newUser = task.Result;

                                Debug.LogFormat("User signed in successfully: {0} ({1})",
                                    newUser.DisplayName, newUser.UserId);

                                _usernameText.text = "Display name: " + newUser.DisplayName + ", " + "Id: " + newUser.UserId;
                                _loginStateText.text = "Logged in";
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

    public void LoginAnonymously()
    {
        try
        {
            Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);

                _usernameText.text = "Display name: " + result.User.DisplayName + ", " + "Id: " + result.User.UserId;
                _loginStateText.text = "Logged in anonymously";
            });
        }
        catch (Exception e)
        {
            _loginStateText.text = e.Message;
            Debug.Log(e);
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
                                _loginStateText.text = "Login canceled";
                            }
                            else if (task.IsFaulted)
                            {
                                _loginStateText.text = "Login faulted";
                            }
                            else
                            {
                                _loginStateText.text = "Logged in";

                                Firebase.Auth.AuthResult result = task.Result;
                                Debug.LogFormat("User signed in successfully: {0} ({1})",
                                    result.User.DisplayName, result.User.UserId);


                                // Get auth's user profile
                                Firebase.Auth.FirebaseUser user = auth.CurrentUser;
                                if (user != null)
                                {
                                    string newDisplayName = PlayGamesPlatform.Instance.GetUserDisplayName();
                                    // Create a new user profile
                                    Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
                                    {
                                        DisplayName = newDisplayName,
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

                                        _usernameText.text = "Display name: " + profile.DisplayName + ", " + "Id: " + user.UserId;
                                        _loginStateText.text = "Logged in, converted from anonymous!";
                                    });

                                }
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
}

