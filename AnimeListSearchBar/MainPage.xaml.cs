using Newtonsoft.Json;
using Org.W3c.Dom;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AnimeListSearchBar;


public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    private List<Anime> _animes;

    public event PropertyChangedEventHandler PropertyChanged;
    public Command<Anime> ShowDetailsCommand { get; }
    public List<Anime> Animes
    {
        get => _animes;
        set
        {
            _animes = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Animes)));
        }
    }

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
        ShowDetailsCommand = new Command<Anime>(async (animeResult) => await ShowDetails(animeResult));

    }

    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"https://api.jikan.moe/v4/anime?q={SearchBar.Text}&sfw");
        var content = await response.Content.ReadAsStringAsync();
        var searchResult = JsonConvert.DeserializeObject<AnimeSearchResult>(content);

        Animes = searchResult.Data;
    }

    private async Task ShowDetails(Anime anime)
    {
        Console.WriteLine(anime.Title);
        await DisplayAlert("Alert", anime.Title, "OK");
        //await Navigation.PushAsync(new AnimeDetailsPage(anime));
    }

    private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
        {
            return;
        }

        var selectedAnime = e.SelectedItem as Anime;
        //await Navigation.PushAsync(new AnimeDetailsPage(selectedAnime));
        //ListView.SelectedItem = null;
    }

}

public class Anime
{
    public string Url { get; set; }
    public string Synopsis { get; set; }
    public string Type { get; set; }
    public string Episodes { get; set; }
    public float? Score { get; set; }

    [JsonProperty("mal_id")]
    public int Id { get; set; }

    [JsonProperty("images")]
    public Image images { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    public string ImageUrl
    {
        get =>
            images.jpg.small_image_url;
        set
        {}
    }


    [JsonProperty("main_picture")]
    public MainPicture MainPicture { get; set; }
}

public class AnimeSearchResult
{
    [JsonProperty("data")]
    public List<Anime> Data { get; set; }
}
public class Image
{
    [JsonProperty("jpg")]
    public Jpeg jpg { get; set; }
}
public class Jpeg
{
    [JsonProperty("small_image_url")]
    public string small_image_url { get; set; }
}
public class MainPicture
{
    [JsonProperty("medium")]
    public string Medium { get; set; }
}

