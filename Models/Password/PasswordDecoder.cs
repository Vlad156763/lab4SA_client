namespace lab4.Models {
    public static class PasswordDecoder {
        public static string set(string msg) {
            if (msg.Length <= 1)
                return msg;
            return msg[0] + msg[^1].ToString() + set(msg[1..^1]);
        }
    }
}