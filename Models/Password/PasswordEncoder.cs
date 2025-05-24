using System.Collections.Generic;
using System.Linq;

namespace lab4.Models {
    public static class PasswordEncoder {
        public static string set(string msg) {
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