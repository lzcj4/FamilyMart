
namespace FamilyMartUI.ViewModel
{
    interface ISearch
    {
        string SearchKeyword { get; }
        bool IsSearchEnabled { get; }
        string NotFoundHint { get; }

        void Search(string keyword);
        void Cancel();
    }
}
