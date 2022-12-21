using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface;
using ImGuiNET;
using MareSynchronos.API;

namespace MareSynchronos.UI.Component
{
    /// <summary>
    /// Component to render a pair category in the mare sync list.
    /// </summary>
    public class PairCategoryUi
    {
        private readonly String _folderName;
        private readonly bool _open;
        private readonly Action<bool> _openChange;
        private readonly List<ClientPairDto> _clientPairDtos;
        private readonly Action<ClientPairDto> _clientRenderFn;
        
        public PairCategoryUi(string folderName,
            bool open,
            Action<bool> openChange,
            List<ClientPairDto> clientPairDtos,
            Action<ClientPairDto> clientRenderFn)
        {
            _folderName = folderName;
            _open = open;
            _openChange = openChange;
            _clientPairDtos = clientPairDtos;
            _clientRenderFn = clientRenderFn;
        }

        public void Draw()
        {
            var resultFolderName = $"{_folderName} ({_clientPairDtos.Count})";

            // Draw the folder icon
            UiShared.FontTextUnformatted(FontAwesomeIcon.Folder.ToIconString(), UiBuilder.IconFont);
            ImGui.SameLine();
            UiShared.FontTextUnformatted(resultFolderName, UiBuilder.DefaultFont);
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                _openChange.Invoke(!_open);
            }

            UiShared.AttachToolTip("This is your group");

            if (_open)
            {
                _clientPairDtos.ToList().ForEach(clientPair =>
                {
                    // This is probably just dumb. Somehow, just setting the cursor position to the icon lenght
                    // does not really push the child rendering further. So we'll just add two whitespaces and call it a day?
                    UiShared.FontTextUnformatted("    ", UiBuilder.DefaultFont);
                    ImGui.SameLine();
                    _clientRenderFn(clientPair);
                });
            }
        }
    }
}