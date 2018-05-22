using Meticulos.Api.App.Fields;
using Meticulos.Api.App.Items;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meticulos.Api.App.WorkflowFunctions.PostFunctions
{
    public class SetFieldValuePostFunction : WorkflowFunction, IWorkflowFunction
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly IItemRepository _itemRepository;
        private readonly Item _item;

        public SetFieldValuePostFunction(IFieldRepository fieldRepository,
            IItemRepository itemRepository, Item item)
        {
            _fieldRepository = fieldRepository;
            _itemRepository = itemRepository;
            _item = item;
        }

        public Task<OperationResult<bool>> Execute(string argsObject)
        {
            return Task.Run(async () =>
            {
                if (_item == null)
                {
                    return new OperationResult<bool>()
                    {
                        Value = false,
                        ErrorMessage = "Item is required."
                    };
                }

                SetFieldValuePostFunctionArgs args =
                    JsonConvert.DeserializeObject<SetFieldValuePostFunctionArgs>(argsObject);

                if (args == null || args.FieldId == null || args.FieldValue == null)
                {
                    return new OperationResult<bool>()
                    {
                        Value = false,
                        ErrorMessage = "Post-function is not configured correctly."
                    };
                }

                List<FieldValue> fieldValues = _item.FieldValues;

                if (fieldValues == null)
                    fieldValues = new List<FieldValue>();

                // Get field information
                ObjectId fieldId = new ObjectId(args.FieldId);
                var fieldInfo = await _fieldRepository.Get(fieldId);

                // Set field value on item
                var fieldValue = fieldValues.Where(v => v.FieldId == fieldId).FirstOrDefault();

                if (fieldValue == null)
                {   // Add field value to item
                    _item.FieldValues.Add(new FieldValue()
                    {
                        FieldId = fieldId,
                        FieldName = fieldInfo.Name,
                        Value = args.FieldValue
                    });
                }
                else
                {   // Update field value to item
                    fieldValue.FieldName = fieldInfo.Name;
                    fieldValue.Value = args.FieldValue;
                }

                var updateResult = await _itemRepository.Update(_item);

                return new OperationResult<bool>() { Value = true };
            });
        }
    }

    public class SetFieldValuePostFunctionArgs
    {
        public string FieldId { get; set; }
        public string FieldValue { get; set; }
    }
}
