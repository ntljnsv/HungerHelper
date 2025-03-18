using Recipes.Models.Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Mvc;

namespace Recipes.ModelBinders
{
    public class ContentModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var ModelName = bindingContext.ModelName;
            var key = $"{ModelName}.ContentType";
            
            var ContentTypeValue = bindingContext.ValueProvider.GetValue(key)?.AttemptedValue;

            Type ConcreteType = null;
            if(ContentTypeValue == "Text")
            {
                ConcreteType = typeof(RecipeText);
            }
            else if(ContentTypeValue == "Photo")
            {
                ConcreteType = typeof(RecipePhoto);
            }
            
            if(ConcreteType == null)
            {
                return base.BindModel(controllerContext, bindingContext);
            }

            var instance = Activator.CreateInstance(ConcreteType);
            var newBindingContext = new ModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => instance, ConcreteType),
                ModelName = bindingContext.ModelName,
                ModelState = bindingContext.ModelState,
                ValueProvider = bindingContext.ValueProvider,
                PropertyFilter = bindingContext.PropertyFilter
            };

            return base.BindModel(controllerContext, newBindingContext);

        }
    }
}