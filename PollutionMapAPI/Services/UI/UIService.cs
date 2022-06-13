using PollutionMapAPI.Data.Entities;
using PollutionMapAPI.Repositories;
using System.Text.Json;

namespace PollutionMapAPI.Services.UI;

public class UIService : IUIService
{
    private readonly IUnitOfWork _unitOfWork;

    public UIService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Data.Entities.UI?> GetUIByIdAsync(Guid id, Guid userId)
    {
        var ui = await _unitOfWork.UIRepository.GetByIdAsync(id);

        if(ui is null || ui.Map.UserId != userId)
            // user has no access to the UI
            return null;

        return ui;
    }

    public async Task<List<DatasetProperty>> FindPropertiesAsync(Data.Entities.UI mapUI, IEnumerable<string> propertiesNames)
    {
        var properties = await _unitOfWork.DatasetPropertyRepository.GetWhereAsync(
            property => property.DataSetId == mapUI.Map.DatasetId && 
            propertiesNames.Contains(property.PropertyName)
        );

        return properties.ToList();
    }

    public async Task<UIElement> CreateUIElementAsync(Data.Entities.UI ui, UIElementType type, List<DatasetProperty> properties, JsonElement style)
    {
        var newUiElement = new UIElement()
        {
            UIId = ui.Id,
            UI = ui,
            Type = type,
            Style = style.ToString(),
            Properties = properties,
        };

        _unitOfWork.UIElementRepository.Add(newUiElement);
        await _unitOfWork.SaveChangesAsync();

        return newUiElement;
    }

    public async Task<UIElement?> GetUIElementByIdAsync(Guid uiElementId, Guid userId)
    {
        var storedUIElement = await _unitOfWork.UIElementRepository.FirstOrDefaultAsync(item => item.Id == uiElementId);
        if (storedUIElement is null)
            // UI Element does not exist
            return null;

        var dataset = await GetUIByIdAsync(storedUIElement.UIId, userId);
        if (dataset is null)
            // UI Element has no access to the element's UI
            return null;

        return storedUIElement;
    }

    public async Task<UIElement> UpdateUIElementAsync(UIElement uiElement, UIElementType type, List<DatasetProperty> properties, JsonElement style)
    {
        uiElement.Type = type;
        uiElement.Style = style.ToString();

        // Delete link to properties not requested in the 'properties' list
        var removedProperty = uiElement.Properties.Where(prop => !properties.Contains(prop)).ToList();
        foreach (var property in removedProperty)
        {
            property.UIElements.Remove(uiElement);
            _unitOfWork.DatasetPropertyRepository.Update(property);
        }

        uiElement.Properties = properties;

        _unitOfWork.UIElementRepository.Update(uiElement);
        await _unitOfWork.SaveChangesAsync();

        return uiElement;
    }

    public Task DeleteUIElementAsync(UIElement uiElement)
    {
        _unitOfWork.UIElementRepository.Remove(uiElement);
        return _unitOfWork.SaveChangesAsync();
    }
}


public static class UIElementExtensions
{
    /// <summary>
    /// Does UI Element Type accept such related properties
    /// </summary>
    public static bool IsValidForProperties(this UIElementType uiElementType, List<DatasetProperty> relatedProperties, out string? errorMessage)
    {
        errorMessage = null;

        //if (!relatedProperties.Any())
        //{
        //    errorMessage = "Should have at least one related property";
        //    return false;
        //}

        if (uiElementType == UIElementType.Slider)
        {
            if (relatedProperties.Count > 1)
            {
                errorMessage = "Slider should not have more than one related property";
                return false;
            }

            var propertiesTypes = relatedProperties.Select(p => p.PropertyType).ToArray();

            return true;
        }

        if (uiElementType == UIElementType.CategorySelector)
        {
            if (relatedProperties.Count > 1)
            {
                errorMessage = "CategorySelector should not have more than one related property";
                return false;
            }

            var propertiesTypes = relatedProperties.Select(p => p.PropertyType).ToArray();

            if (!propertiesTypes.All(t => t == DatasetPropertyType.Category))
            {
                errorMessage = "CategorySelector accepts only Category properties";
                return false;
            }

            return true;
        }

        if (uiElementType == UIElementType.Toggle)
        {
            if (relatedProperties.Count > 1)
            {
                errorMessage = "Toogle should not have more than one related property";
                return false;
            }

            var propertiesTypes = relatedProperties.Select(p => p.PropertyType).ToArray();

            if (!propertiesTypes.All(t => t == DatasetPropertyType.Bool))
            {
                errorMessage = "Toogle accepts only Bool properties";
                return false;
            }

            return true;
        }

        if (uiElementType == UIElementType.DateTimePicker)
        {
            if (relatedProperties.Count > 1)
            {
                errorMessage = "DateTimePicker should not have more than one related property";
                return false;
            }

            var propertiesTypes = relatedProperties.Select(p => p.PropertyType).ToArray();

            if (!propertiesTypes.All(t => t == DatasetPropertyType.DateTime))
            {
                
                errorMessage = "DateTimePicker accepts only DateTime properties";
                return false;
            }

            return true;
        }

        if (uiElementType == UIElementType.Isoline)
        {
            var propertiesTypes = relatedProperties.Select(p => p.PropertyType).ToArray();

            if (!propertiesTypes.All(t => t == DatasetPropertyType.Number))
            {
                errorMessage = "Isoline accepts only Number properties";
                return false;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Is this string a valid json
    /// </summary>
    public static bool IsJson(this string str, out string? errorMessage)
    {
        errorMessage = null;

        str = str.Trim();
        try
        {
            using var obj = JsonDocument.Parse(str);
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }
}

public static class UIServiceExtensions
{
    public static IServiceCollection AddUIService(this IServiceCollection services)
    {
        services.AddTransient<IUIService, UIService>();
        return services;
    }
}
