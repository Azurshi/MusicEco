using System.Collections.ObjectModel;

namespace PerformanceTest;

public class MainPageViewModel {
    public ObservableCollection<ItemViewModel> Items { get; init; }
    public MainPageViewModel() {
        this.Items = [];
        for(int i=0; i<200; i++) {
            this.Items.Add(new($"Item {i+1}"));
        }
    }
}
