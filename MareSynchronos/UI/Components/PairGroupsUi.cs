using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using MareSynchronos.API;
using MareSynchronos.UI.Handlers;
using MareSynchronos.WebAPI;

namespace MareSynchronos.UI.Components
{
    public class PairGroupsUi
    {
        private readonly Action<ClientPairDto> _clientRenderFn;
        private readonly TagHandler _tagHandler;

        public PairGroupsUi(TagHandler tagHandler, Action<ClientPairDto> clientRenderFn)
        {
            _clientRenderFn = clientRenderFn;
            _tagHandler = tagHandler;
        }

        public void Draw(List<ClientPairDto> availablePairs)
        {
            foreach (var tag in _tagHandler.GetAllTagsSorted())
            {
                UiShared.DrawWithID($"group-{tag}", () => DrawCategory(tag, availablePairs));
            }
        }

        public void DrawCategory(string tag, List<ClientPairDto> availablePairs)
        {
            DrawName(tag);
            DrawButtons(tag);
            if (_tagHandler.IsTagOpen(tag))
            {
                DrawPairs(tag, availablePairs);
            }
        }

        private void DrawName(string tag)
        {
            var resultFolderName = $"{tag}";

            // Draw the folder icon
            UiShared.FontTextUnformatted(FontAwesomeIcon.Folder.ToIconString(), UiBuilder.IconFont);
            ImGui.SameLine();
            UiShared.FontTextUnformatted(resultFolderName, UiBuilder.DefaultFont);
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                ToggleTagOpen(tag);
            }
        }

        private void DrawButtons(string tag)
        {
            var buttonX = UiShared.GetIconButtonSize(FontAwesomeIcon.Trash).X;
            var windowX = ImGui.GetWindowContentRegionMin().X;
            var windowWidth = UiShared.GetWindowContentRegionWidth();

            var buttonDeleteOffset = windowX + windowWidth - buttonX;
            ImGui.SameLine(buttonDeleteOffset);
            if (ImGuiComponents.IconButton(FontAwesomeIcon.Trash))
            {
                _tagHandler.RemoveTag(tag);
            }

            UiShared.AttachToolTip($"Delete Group {tag} (Will not delete the pairs)");
        }

        private void DrawPairs(string tag, List<ClientPairDto> availablePairs)
        {
            // These are all the OtherUIDs that are tagged with this tag
            var otherUidsTaggedWithTag = _tagHandler.GetOtherUidsForTag(tag);
            availablePairs
                .Where(pair => otherUidsTaggedWithTag.Contains(pair.OtherUID))
                .ToList()
                .ForEach(clientPair =>
                {
                    // This is probably just dumb. Somehow, just setting the cursor position to the icon lenght
                    // does not really push the child rendering further. So we'll just add two whitespaces and call it a day?
                    UiShared.FontTextUnformatted("    ", UiBuilder.DefaultFont);
                    ImGui.SameLine();
                    _clientRenderFn(clientPair);
                });
        }

        private void ToggleTagOpen(string tag)
        {
            bool open = !_tagHandler.IsTagOpen(tag);
            _tagHandler.SetTagOpen(tag, open);
        }
    }
}