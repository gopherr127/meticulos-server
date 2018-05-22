using Meticulos.Api.App.Fields;
using Meticulos.Api.App.Items;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowFunctions.Validations
{
    public class FieldRequiredValidation : WorkflowFunction, IWorkflowFunction
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly Item _item;

        public FieldRequiredValidation(
            IFieldRepository fieldRepository,
            Item item)
        {
            _fieldRepository = fieldRepository;
            _item = item;
        }

        public Task<OperationResult<bool>> Execute(string argsObject)
        {
            return Task.Run( async () =>
            {
                if (_item == null)
                {
                    return new OperationResult<bool>()
                    {
                        Value = false,
                        ErrorMessage = "Item is required."
                    };
                }

                List<string> validationErrors = new List<string>();

                FieldRequiredValidationArgs args =
                    JsonConvert.DeserializeObject<FieldRequiredValidationArgs>(argsObject);

                if (args == null || args.RequiredFieldIds == null)
                {
                    return new OperationResult<bool>()
                    {
                        Value = false,
                        ErrorMessage = "Validation is not configured correctly."
                    };
                }

                List<FieldValue> fieldValues = _item.FieldValues;

                if (fieldValues == null)
                    fieldValues = new List<FieldValue>();

                foreach (string requiredFieldId in args.RequiredFieldIds)
                {
                    var fieldValue = fieldValues
                        .Where(v => v.FieldId == new ObjectId(requiredFieldId)).FirstOrDefault();

                    if (fieldValue == null || string.IsNullOrEmpty(fieldValue.Value))
                    {
                        if (fieldValue == null)
                        {   // Field isn't on the item at all; need to pull info for it
                            var fieldInfo = await _fieldRepository.Get(new ObjectId(requiredFieldId));
                            validationErrors.Add(fieldInfo.Name + " is required");
                        }
                        else
                        {
                            validationErrors.Add(fieldValue.FieldName + " is required");
                        }
                    }
                }

                if (validationErrors.Count == 0)
                {
                    return new OperationResult<bool>() { Value = true };
                }
                else
                {
                    return new OperationResult<bool>()
                    {
                        Value = false,
                        ErrorMessage = string.Join('\n', validationErrors)
                    };
                }
            });
        }
    }

    public class FieldRequiredValidationArgs
    {
        public List<string> RequiredFieldIds { get; set; }
    }
}
