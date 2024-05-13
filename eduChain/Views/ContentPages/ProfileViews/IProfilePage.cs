namespace eduChain;

public interface IProfilePage
{
    void Back(object sender, EventArgs e);
    void EditProfile(object sender, EventArgs e);
    void CancelEditProfile(object sender, EventArgs e);
    void SaveChanges(object sender, EventArgs e);
    void ShowPersonal(object sender, EventArgs e);
    void HidePersonal(object sender, EventArgs e);
    void ShowForm(object sender, EventArgs e);


}
