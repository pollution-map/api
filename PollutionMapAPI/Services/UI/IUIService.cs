using PollutionMapAPI.Data.Entities;
using System.Text.Json;

namespace PollutionMapAPI.Services.UI;

public interface IUIService
{
    Task<Data.Entities.UI?> GetUIByIdAsync(Guid value, Guid userId);
    Task<UIElement> CreateUIElementAsync(Data.Entities.UI ui, UIElementType type, List<DatasetProperty> properties, JsonElement style);
    Task<List<DatasetProperty>> FindPropertiesAsync(Data.Entities.UI ui, IEnumerable<string> propertiesNames);
    Task <UIElement?> GetUIElementByIdAsync(Guid uiElementId, Guid userId);
    Task<UIElement> UpdateUIElementAsync(UIElement uIElement, UIElementType type, List<DatasetProperty> properties, JsonElement style);
    Task DeleteUIElementAsync(UIElement uiElement);
}
