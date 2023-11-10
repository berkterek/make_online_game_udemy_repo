using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeOnlineGame.Networks.Shares
{
    [System.Serializable]
    public struct UserData
    {
        public string UserName;
        public string UserID;
        public GameInfo UserGamePreferences;
    }

    [System.Serializable]
    public struct GameInfo
    {
        public Map Map;
        public GameMode GameMode;
        public GameQueue GameQueue;

        public string ToMultiplayerQueue()
        {
            string gameQueue;
            switch (GameQueue)
            {
                case GameQueue.Solo:
                    gameQueue = "solo-queue";
                    break;
                case GameQueue.Team:
                    gameQueue = "team-queue";
                    break;
                default:
                    gameQueue = "none";
                    break;
            }

            return gameQueue;
        }
    }

    public enum Map : byte
    {
        Default,
    }

    public enum GameMode : byte
    {
        Default,
    }

    public enum GameQueue : byte
    {
        Solo,
        Team
    }
}