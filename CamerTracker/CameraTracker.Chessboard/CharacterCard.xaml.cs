using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CameraTracker.Chessboard
{
    /// <summary>
    /// Interaction logic for CharacterCard.xaml
    /// </summary>
    public partial class CharacterCard : UserControl
    {
        public CharacterCard()
        {
            InitializeComponent();
        }

        public int Health
        {
            get { return (int)GetValue(HealthProperty); }
            set { SetValue(HealthProperty, value); }
        }

        public static readonly DependencyProperty HealthProperty = DependencyProperty.Register("Health", typeof(int), typeof(CharacterCard), new FrameworkPropertyMetadata(100));

        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(CharacterCard), new FrameworkPropertyMetadata(Brushes.Black));

        public string CharacterName
        {
            get { return (string)GetValue(CharacterNameProperty); }
            set { SetValue(CharacterNameProperty, value); }
        }

        public static readonly DependencyProperty CharacterNameProperty = DependencyProperty.Register("CharacterName", typeof(string), typeof(CharacterCard), new FrameworkPropertyMetadata("Character"));
    }
}
