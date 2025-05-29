namespace lab4.Interface;
public interface IPassword {
    string Decode(string msg);
    string Encode(string msg);
}