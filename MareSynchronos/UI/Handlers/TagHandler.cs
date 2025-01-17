﻿using MareSynchronos.API.Dto.User;
using MareSynchronos.Managers;

namespace MareSynchronos.UI.Handlers
{
    public class TagHandler
    {
        private readonly ServerConfigurationManager _serverConfigurationManager;
        public const string CustomVisibleTag = "Mare_Visible";
        public const string CustomOnlineTag = "Mare_Online";
        public const string CustomOfflineTag = "Mare_Offline";

        public TagHandler(ServerConfigurationManager serverConfigurationManager)
        {
            _serverConfigurationManager = serverConfigurationManager;
        }

        public void AddTag(string tag)
        {
            _serverConfigurationManager.AddTag(tag);
        }

        public void RemoveTag(string tag)
        {
            // First remove the tag from teh available pair tags
            _serverConfigurationManager.RemoveTag(tag);
        }

        public void SetTagOpen(string tag, bool open)
        {
            if (open)
            {
                _serverConfigurationManager.AddOpenPairTag(tag);
            }
            else
            {
                _serverConfigurationManager.RemoveOpenPairTag(tag);
            }
        }

        /// <summary>
        /// Is this tag opened in the paired clients UI?
        /// </summary>
        /// <param name="tag">the tag</param>
        /// <returns>open true/false</returns>
        public bool IsTagOpen(string tag)
        {
            return _serverConfigurationManager.ContainsOpenPairTag(tag);
        }

        public List<string> GetAllTagsSorted()
        {
            return _serverConfigurationManager.GetServerAvailablePairTags()
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public HashSet<string> GetOtherUidsForTag(string tag)
        {
            return _serverConfigurationManager.GetUidsForTag(tag);
        }

        public void AddTagToPairedUid(UserPairDto pair, string tagName)
        {
            _serverConfigurationManager.AddTagForUid(pair.User.UID, tagName);
        }

        public void RemoveTagFromPairedUid(UserPairDto pair, string tagName)
        {
            _serverConfigurationManager.RemoveTagForUid(pair.User.UID, tagName);
        }

        public bool HasTag(UserPairDto pair, string tagName)
        {
            return _serverConfigurationManager.ContainsTag(pair.User.UID, tagName);
        }

        public bool HasAnyTag(UserPairDto pair)
        {
            return _serverConfigurationManager.HasTags(pair.User.UID);
        }
    }
}