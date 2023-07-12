public class HealthBar : Bar
{
    private Player player;


    private void OnEnable()
    {
        player = Player.LocalPlayer;

        player.HealthChanged += OnValueChanged;
        Slider.value = 1;

    }

    private void OnDisable()
    {

        player.HealthChanged -= OnValueChanged;
    }
}
