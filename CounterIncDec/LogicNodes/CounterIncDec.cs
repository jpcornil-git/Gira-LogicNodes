using LogicModule.Nodes.Helpers;
using LogicModule.ObjectModel;
using LogicModule.ObjectModel.TypeSystem;
using System;

namespace jeanphilippe_cornil_gmail_com.Logic.Nodes
{
    public class CounterIncDec : LogicNodeBase
    {

        /// <summary>
        /// Input: Any new telegram increases counter value by 1.
        /// </summary>
        [Input(DisplayOrder = 1, IsInput = true)]
        public BoolValueObject Increase { get; private set; }

        /// <summary>
        /// Input: Any new telegram decreases counter value by 1.
        /// </summary>
        [Input(DisplayOrder = 2, IsInput = true)]
        public BoolValueObject Decrease { get; private set; }

        /// <summary>
        /// Input: Set counter value.
        /// </summary>
        [Input(DisplayOrder = 3, IsInput = true, IsDefaultShown = false)]
        public IntValueObject Set { get; private set; }

        /// <summary>
        /// Parameter: Increment/decrement step size.
        /// </summary>
        [Parameter(InitOrder = 4)]
        public IntValueObject Step { get; private set; }
        
        /// <summary>
        /// Parameter: Minimum counter value.
        /// </summary>
        [Parameter(InitOrder = 5)]
        public IntValueObject Minimum { get; private set; }

        /// <summary>
        /// Parameter: Maximum counter value.
        /// </summary>
        [Parameter(InitOrder = 6)]
        public IntValueObject Maximum { get; private set; }

        /// <summary>
        /// Output: Current counter value.
        /// </summary>
        [Output(DisplayOrder = 1)]
        public IntValueObject Output { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterIncDec"/> class.
        /// </summary>
        /// <param name="context">The node context.</param>
        public CounterIncDec(INodeContext context)
          : base(context)
        {
            context.ThrowIfNull("context");

            var typeService = context.GetService<ITypeService>();
            this.Increase = typeService.CreateBool(PortTypes.Binary, "Increase", false);
            this.Decrease = typeService.CreateBool(PortTypes.Binary, "Decrease", false);
            this.Set      = typeService.CreateInt(PortTypes.Integer, "Set");

            this.Step    = typeService.CreateInt(PortTypes.Integer, "Step", 1);
            this.Minimum = typeService.CreateInt(PortTypes.Integer, "Minimum", 0);
            this.Maximum = typeService.CreateInt(PortTypes.Integer, "Maximum", 1);

            this.Output = typeService.CreateInt(PortTypes.Integer, "Value");
        }
        /// <summary>
        /// Called when logic node is initialized
        /// </summary>
        public override void Startup()
        {
            this.Output.MinValue = this.Minimum;
            this.Output.MaxValue = this.Maximum;
            this.Output.Value = Math.Max(this.Minimum, Math.Min(this.Maximum,this.Set.Value));
        }

        /// <summary>
        /// Called when one or more inputs have been updated
        /// </summary>
        public override void Execute()
        {
            if (this.Set.WasSet && this.Set.HasValue)
            {
                this.Output.Value = this.Set.Value;
            }
            else
            {
                // Default ValueObject behavior is saturation -> no range check required
                if (this.Increase.WasSet)
                    this.Output.Value += this.Step;
                if (this.Decrease.WasSet)
                    this.Output.Value -= this.Step;
            }
        }

        /// <summary>
        /// Called by GPA to check parameter settings and correct node configuration
        /// </summary>
        public override ValidationResult Validate(string language)
        {
            if (this.Minimum > this.Maximum)
            {
                return new ValidationResult
                {
                    HasError = true,
                    Message = this.Localize(language, "MinGreaterThanMaxErrorMessage")
                };
            }
            return base.Validate(language);
        }
    }
}
