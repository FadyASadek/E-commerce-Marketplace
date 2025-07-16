namespace OnlineStore.ViewModel.PublicInfoViewModel
{
    public class AddPublicInfoVM
    {
        public string Title { get; set; } = string.Empty;
        public IFormFile Logo { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
