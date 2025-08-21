namespace CardGames.Core
{
    /// <summary>
    /// Centralized constants for card visual properties to ensure consistency across all UI components
    /// </summary>
    public static class CardVisualConstants
    {
        /// <summary>
        /// Standard width for all card visual elements
        /// </summary>
        public const double CardWidth = 80.0;

        /// <summary>
        /// Standard height for all card visual elements
        /// </summary>
        public const double CardHeight = 120.0;

        /// <summary>
        /// Vertical offset for tableau column card stacking to show rank and suit
        /// Increased by 20% to provide more vertical spacing (was 20.0)
        /// </summary>
        public const double TableauVerticalOffset = 20.0;
    }
}