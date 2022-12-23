using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using MareSynchronos.API;
using MareSynchronos.Models;
using MareSynchronos.Utils;
using MareSynchronos.WebAPI;

namespace MareSynchronos.UI.Component
{
    public class PairCategoriesUi
    {
        private readonly Configuration _configuration;
        private readonly ApiController _apiController;
        private readonly Action<ClientPairDto> _clientRenderFn;

        /// <summary>
        /// When we right-click a category, we toggle it into edit mode.
        /// This keeps track of all the edited names, where the key is the category id.
        /// </summary>
        private readonly Dictionary<string, string> _categoryEditNameForCategoryId = new(System.StringComparer.Ordinal);

        /// <summary>
        /// When we right-click a category, we toggle it into edit mode.
        /// This keeps track of all the active edit modes.
        /// </summary>
        private readonly Dictionary<string, bool> _categoryEditingForCategoryId = new(System.StringComparer.Ordinal);

        private bool _showPairSelectionPopup = false;
        private bool _pairSelectionPopupOpened = false;
        private string _categoryIdPairSelection = "";

        public PairCategoriesUi(Configuration configuration, ApiController apiController, Action<ClientPairDto> clientRenderFn)
        {
            _configuration = configuration;
            _apiController = apiController;
            _clientRenderFn = clientRenderFn;
        }

        public void Draw()
        {
            foreach (var category in _configuration.PairCategories)
            {
                UiShared.DrawWithID(category.CategoryId, () => DrawCategory(category));
            }
        }
        
        public void DrawCategory(PairCategory pairCategory)
        {
            if (IsCategoryEditing(pairCategory.CategoryId))
            {
                DrawEditName(pairCategory);
            }
            else
            {
                DrawName(pairCategory);
            }

            DrawButtons(pairCategory);
            if (pairCategory.Open)
            {
                DrawPairs(pairCategory);
            }

            DrawSelectPairsPopup(pairCategory);
        }

        private void DrawEditName(PairCategory category)
        {
            var nameBefore = GetCategoryEditName(category.CategoryId);
            var nameValue = nameBefore;
            if (ImGui.InputTextWithHint("", "Nick/Notes", ref nameValue, 255, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                // This block is executed 
                RenameCategory(category.CategoryId, nameValue);
                ToggleCategoryEditing(category.CategoryId);
                SetCategoryEditName(category.CategoryId, "");
            }

            if (!nameBefore.Equals(nameValue))
            {
                // Name changed in this ticket, update name in internal state
                SetCategoryEditName(category.CategoryId, nameValue);
            }

            UiShared.AttachToolTip("Hit ENTER to save\nRight click to cancel");
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                ToggleCategoryEditing(category.CategoryId);
                SetCategoryEditName(category.CategoryId, "");
            }
        }

        private void DrawName(PairCategory pairCategory)
        {
            var resultFolderName = $"{pairCategory.CategoryName}";

            // Draw the folder icon
            UiShared.FontTextUnformatted(FontAwesomeIcon.Folder.ToIconString(), UiBuilder.IconFont);
            ImGui.SameLine();
            UiShared.FontTextUnformatted(resultFolderName, UiBuilder.DefaultFont);
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                ToggleCategoryOpen(pairCategory);
            }

            UiShared.AttachToolTip("Right click to edit name. Left-click to open.");
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                ToggleCategoryEditing(pairCategory.CategoryId);
                if (IsCategoryEditing(pairCategory.CategoryId))
                {
                    SetCategoryEditName(pairCategory.CategoryId, pairCategory.CategoryName);
                }
            }
        }

        private void DrawButtons(PairCategory category)
        {
            var buttonSizeEditX = UiShared.GetIconButtonSize(FontAwesomeIcon.User).X;
            var buttonSizeDeleteX = UiShared.GetIconButtonSize(FontAwesomeIcon.Trash).X;
            var windowX = ImGui.GetWindowContentRegionMin().X;
            var windowWidth = UiShared.GetWindowContentRegionWidth();
            ImGui.SameLine(windowX + windowWidth - buttonSizeEditX - buttonSizeDeleteX -
                           ImGui.GetStyle().ItemSpacing.X);
            if (ImGuiComponents.IconButton(FontAwesomeIcon.User))
            {
                _showPairSelectionPopup = true;
                _categoryIdPairSelection = category.CategoryId;
            }

            UiShared.AttachToolTip($"Manage pairs for {category.CategoryName}");


            var buttonDeleteOffset = windowX + windowWidth - buttonSizeEditX;
            ImGui.SameLine(buttonDeleteOffset);
            if (ImGuiComponents.IconButton(FontAwesomeIcon.Trash))
            {
                DeleteCategory(category.CategoryId);
            }

            UiShared.AttachToolTip($"Delete {category.CategoryName} (Will not delete the pairs)");
        }

        private void DrawPairs(PairCategory category)
        {
            var pairedClientsForId = _apiController.PairedClients.ToDictionary(
                pairCategory => pairCategory.OtherUID,
                pairCategory => pairCategory,
                StringComparer.Ordinal
            );
            category.PairPartnerUids
                .Select(pair => pairedClientsForId[pair])
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

        private void DrawSelectPairsPopup(PairCategory category)
        {
            if (category.CategoryId.Equals(_categoryIdPairSelection))
            {
                var title = $"Choose Pairs for {category.CategoryName}";
                // If the modal is supposed to be open but is not opened yet => open it
                if (_showPairSelectionPopup && !_pairSelectionPopupOpened)
                {
                    _pairSelectionPopupOpened = true;
                    ImGui.OpenPopup(title);
                }
                if (!_showPairSelectionPopup)
                {
                    _pairSelectionPopupOpened = false;
                }
            
                if (ImGui.BeginPopupModal(title, ref _showPairSelectionPopup, UiShared.PopupWindowFlags))
                {
                    UiShared.TextWrapped($"Select the pairs you wish to group into {category.CategoryName}");
                
                    foreach (var availablePair in _apiController.PairedClients)
                    {
                        bool checkboxValueBefore = category.PairPartnerUids.Contains(availablePair.OtherUID, StringComparer.Ordinal);
                        bool checkboxValue = category.PairPartnerUids.Contains(availablePair.OtherUID, StringComparer.Ordinal);
                        ImGui.Checkbox(availablePair.OtherUID, ref checkboxValue);
                        if (checkboxValueBefore != checkboxValue)
                        {
                            if (checkboxValue)
                            {
                                category.PairPartnerUids.Add(availablePair.OtherUID);
                            }
                            else
                            {
                                category.PairPartnerUids.Remove(availablePair.OtherUID);
                            }
                            _configuration.Save();
                        }
                    
                    }
                    if (ImGui.Button("Done"))
                    {
                        _showPairSelectionPopup = false;
                    }

                    // UiShared.SetScaledWindowSize(350);
                    UiShared.SetScaledWindowSize(400);
                    ImGui.EndPopup();
                }
            }
        }
        

        private bool IsCategoryEditing(string categoryId)
        {
            return _categoryEditingForCategoryId.GetValueOrDefault(categoryId, false);
        }

        private void ToggleCategoryEditing(string categoryId)
        {
            bool newValue = !IsCategoryEditing(categoryId);
            _categoryEditingForCategoryId[categoryId] = newValue;
        }

        private void SetCategoryEditName(string categoryId, string name)
        {
            _categoryEditNameForCategoryId[categoryId] = name;
        }

        private string GetCategoryEditName(string categoryId)
        {
            return _categoryEditNameForCategoryId[categoryId];
        }

        private void ToggleCategoryOpen(PairCategory category)
        {

            category.Open = !category.Open;
            _configuration.Save();
        }
        
        private void RenameCategory(string categoryId, string newName)
        {
            Logger.Info($"Renaming category {categoryId} to {newName}");
            _configuration.PairCategories.ForEach(category =>
            {
                if (string.Equals(category.CategoryId, categoryId, StringComparison.Ordinal))
                {
                    category.CategoryName = newName;
                }
            });
            _configuration.Save();
        }

        private void DeleteCategory(string categoryId)
        {
            Logger.Info("Removing category " + categoryId);
            _configuration.PairCategories.RemoveAll(category => string.Equals(category.CategoryId, categoryId, StringComparison.Ordinal));
            _configuration.Save();
        }
    }
}