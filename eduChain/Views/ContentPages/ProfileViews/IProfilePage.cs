namespace eduChain;

public interface IProfilePage
{
    void Back(object sender, EventArgs e);
    void EditProfile(object sender, EventArgs e);
    void CancelEditProfile(object sender, EventArgs e);
    void SaveChanges(object sender, EventArgs e);

}
