using lab4.Interface;
using System.Collections.Generic;
using System.Linq;

namespace lab4.Models {
    public class PasswordComponent : IPassword{
        public string Decode(string msg) {
            if (msg.Length <= 1)
                return msg;
            return msg[0] + msg[^1].ToString() + Decode(msg[1..^1]);
        }
        public string Encode(string msg) {
            var part1 = new List<char>();
            var part2 = new List<char>();
            for (int i = 0; i < msg.Length; i++) {
                if (i % 2 == 0)
                    part1.Add(msg[i]);
                else
                    part2.Add(msg[i]);
            }
            part2.Reverse();
            return new string(part1.Concat(part2).ToArray());
        }
    }
}