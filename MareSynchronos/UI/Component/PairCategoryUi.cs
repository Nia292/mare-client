using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using MareSynchronos.API;
using MareSynchronos.Models.DTO;

namespace MareSynchronos.UI.Component
{
    /// <summary>
    /// Component to render a pair category in the mare sync list.
    /// </summary>
    public class PairCategoryUi
    {
        public string CategoryId { get; }
        
  
        private readonly Action _delete;
        private readonly Action<string> _rename;
        private readonly List<ClientPairDto> _clientPairDtos;
        private readonly Action<ClientPairDto> _clientRenderFn;

        /// <summary>
        /// Internal state => is this category open and displays per, yes or no?
        /// </summary>
        private bool _open = false;

        private bool _editing = false;
        
        /// <summary>
        /// If in edit mode, the new name of the category
        /// </summary>
        private string _newName = String.Empty;
        
        private string _folderName;

        public PairCategoryUi(PairCategoryDto pairCategoryDto,
            Action delete,
            Action<string> rename,
            List<ClientPairDto> clientPairDtos,
            Action<ClientPairDto> clientRenderFn)
        {
            _folderName = pairCategoryDto.CategoryName;
            _delete = delete;
            _rename = rename;
            _clientPairDtos = clientPairDtos;
            _clientRenderFn = clientRenderFn;
            CategoryId = pairCategoryDto.CategoryID;
        }

        public void Draw()
        {
            if (_editing)
            {
                DrawEditName();
            }
            else
            {
                DrawName();
            }
            DrawDeleteButton();
            if (_open)
            {
                DrawPairs();
            }
        }

        private void DrawName()
        {
            var resultFolderName = $"{_folderName} ({_clientPairDtos.Count})";

            // Draw the folder icon
            UiShared.FontTextUnformatted(FontAwesomeIcon.Folder.ToIconString(), UiBuilder.IconFont);
            ImGui.SameLine();
            UiShared.FontTextUnformatted(resultFolderName, UiBuilder.DefaultFont);
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                _open = !_open;
            }
            UiShared.AttachToolTip("Right click to edit name. Left-click to open.");
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                _editing = !_editing;
                if (_editing)
                {
                    _newName = _folderName;
                }
            }
        }

        private void DrawEditName()
        {
            if (ImGui.InputTextWithHint("", "Nick/Notes", ref _newName, 255, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                _rename(_newName);
                _folderName = _newName;
                _editing = false;
                _newName = "";
            }
            UiShared.AttachToolTip("Hit ENTER to save\nRight click to cancel");
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                _editing = false;
                _newName = "";
            }
        }

        private void DrawDeleteButton()
        {
            var buttonSize = UiShared.GetIconButtonSize(FontAwesomeIcon.Trash);
            ImGui.SameLine(ImGui.GetWindowContentRegionMin().X + UiShared.GetWindowContentRegionWidth() - buttonSize.X);
            if (ImGuiComponents.IconButton(FontAwesomeIcon.Trash))
            {
                _delete();
            }
            UiShared.AttachToolTip($"Delete {_folderName} (Will not delete the pairs)");
        }

        private void DrawPairs()
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