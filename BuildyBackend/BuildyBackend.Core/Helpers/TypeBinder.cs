using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace BuildyBackend.Core.Helpers
{
    public class TypeBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyName = bindingContext.ModelName;
            var valueProvider = bindingContext.ValueProvider.GetValue(propertyName);
            if (valueProvider == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            try
            {
                var decentralizedValue = JsonConvert.DeserializeObject<List<int>>(valueProvider.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(decentralizedValue);
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(propertyName, "Valor inválido para tipo List<int>");
            }
            return Task.CompletedTask;
        }

    }
}