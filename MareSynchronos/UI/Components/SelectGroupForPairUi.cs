using System.Collections.Generic;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiNET;
using MareSynchronos.API;
using MareSynchronos.UI.Handlers;

namespace MareSynchronos.UI.Components
{
    public class SelectGroupForPairUi
    {
        /// <summary>
        /// Is the UI open or closed?
        /// </summary>
        private bool _open;

        private bool _opened;

        /// <summary>
        /// The group UI is always open for a specific pair. This defines which pair the UI is open for.
        /// </summary>
        /// <returns></returns>
        private ClientPairDto? _pair;

        /// <summary>
        /// For the add category option, this stores the currently typed in tag name
        /// </summary>
        private string _tagNameToAdd = "";
        
        private readonly TagHandler _tagHandler;
        private readonly Configuration _configuration;

        public SelectGroupForPairUi(TagHandler tagHandler, Configuration configuration)
        {
            _open = false;
            _pair = null;
            _tagHandler = tagHandler;
            _configuration = configuration;
        }

        public void Open(ClientPairDto pair)
        {
            _pair = pair;
            _open = true;
        }


        public void Draw(Dictionary<string, bool> showUidForEntry)
        {
            if (_pair == null)
            {
                return;
            }
            if (_open && !_opened)
            {
                ImGui.OpenPopup("Grouping Popup");
                _opened = true;
            }

            if (!_open)
            {
                _opened = false;
            }

            if (ImGui.BeginPopup("Grouping Popup"))
            {
                
                showUidForEntry.TryGetValue(_pair.OtherUID, out var showUidInsteadOfName);
                _configuration.GetCurrentServerUidComments().TryGetValue(_pair.OtherUID, out var playerText);
                if (showUidInsteadOfName)
                {
                    playerText = _pair.OtherUID;
                }
                else if (string.IsNullOrEmpty(playerText))
                {
                    playerText = _pair.OtherUID;
                }
                UiShared.FontTextUnformatted("Groups for " + playerText, UiBuilder.DefaultFont);
                foreach (var tag in _tagHandler.GetAllTagsSorted())
                {
                    UiShared.DrawWithID($"groups-pair-{_pair.OtherUID}-{tag}", () => DrawGroupName(_pair, tag));
                }
                
                if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus))
                {
                    HandleAddTag();
                }
                ImGui.SameLine();
                ImGui.InputTextWithHint("##category_name", "New Group", ref _tagNameToAdd, 40);
                {
                    if (ImGui.IsKeyDown(ImGuiKey.Enter))
                    {
                        HandleAddTag();
                    }
                }

                ImGui.EndPopup();
            }
            else
            {
                _open = false;
            }
        }
        
        private void DrawGroupName(ClientPairDto pair, string name)
        {
            bool hasTagBefore = _tagHandler.HasTag(pair, name);
            bool hasTag = hasTagBefore;
            ImGui.Checkbox(name, ref hasTag);
            if (hasTagBefore != hasTag)
            {
                if (hasTag)
                {
                    _tagHandler.AddTagToPairedUid(pair, name);
                }
                else
                {
                    _tagHandler.RemoveTagFromPairedUid(pair, name);
                }
            }
        }

        private void HandleAddTag()
        {
            if (!_tagNameToAdd.IsNullOrWhitespace())
            {
                _tagHandler.AddTag(_tagNameToAdd);
                _tagNameToAdd = string.Empty;
            }
        }

    }
}