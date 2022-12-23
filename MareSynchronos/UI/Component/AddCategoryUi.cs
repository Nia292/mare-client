using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using MareSynchronos.Models;
using MareSynchronos.Utils;

namespace MareSynchronos.UI.Component
{
    public class AddCategoryUi
    {
        /// <summary>
        /// Local state: The category name
        /// </summary>
        private string _categoryName = string.Empty;

        private readonly Configuration _configuration;

        public AddCategoryUi(Configuration configuration)
        {
            _configuration = configuration;
        }


        public void Draw()
        {
            var buttonSize = UiShared.GetIconButtonSize(FontAwesomeIcon.Plus);
            ImGui.SetNextItemWidth(UiShared.GetWindowContentRegionWidth() - ImGui.GetWindowContentRegionMin().X - buttonSize.X);
            ImGui.InputTextWithHint("##category_name", "Category Name", ref _categoryName, 20);
            ImGui.SameLine(ImGui.GetWindowContentRegionMin().X + UiShared.GetWindowContentRegionWidth() - buttonSize.X);
            if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus))
            {
                AddCategory(_categoryName);
                _categoryName = string.Empty;
            }
            UiShared.AttachToolTip($"Create new category {_categoryName}");

            ImGuiHelpers.ScaledDummy(2);
        }
        
        /// <summary>
        /// Called when a new sync pair category is supposed to be created 
        /// </summary>
        /// <param name="newCategoryName">name of the new category.</param>
        private void AddCategory(string newCategoryName)
        {
            Logger.Debug($"Adding category with name {newCategoryName}");
            var categoryToAdd = new PairCategory(newCategoryName);
            _configuration.PairCategories.Add(categoryToAdd);
            _configuration.Save();
        }
    }
}