using System;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace MareSynchronos.UI.Component
{
    public class AddCategoryUi
    {
        /// <summary>
        /// Local state: The category name
        /// </summary>
        private string _categoryName = string.Empty;

        private Action<string> _addCategoryHandler;

        public AddCategoryUi()
        {
            _addCategoryHandler = _ => {};
        }

        public AddCategoryUi OnAddCategory(Action<string> addCategoryHandler)
        {
            _addCategoryHandler = addCategoryHandler;
            return this;
        }

        public void Draw()
        {
            var buttonSize = UiShared.GetIconButtonSize(FontAwesomeIcon.Plus);
            ImGui.SetNextItemWidth(UiShared.GetWindowContentRegionWidth() - ImGui.GetWindowContentRegionMin().X - buttonSize.X);
            ImGui.InputTextWithHint("##category_name", "Category Name", ref _categoryName, 20);
            ImGui.SameLine(ImGui.GetWindowContentRegionMin().X + UiShared.GetWindowContentRegionWidth() - buttonSize.X);
            if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus))
            {
                _addCategoryHandler(_categoryName);
            }
            UiShared.AttachToolTip($"Create new category {_categoryName}");

            ImGuiHelpers.ScaledDummy(2);
        }
    }
}