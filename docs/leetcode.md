# LeetCode Log

| Day | Problem | Approach | Time Complexity | Space Complexity | Edge Cases | Lesson Learned |
|---|---------|----------|-----------------|------------------|------------|----------------|
| 1 | [Concatenation of Array](https://leetcode.com/problems/concatenation-of-array/) | Create new array of length 2n, copy original twice using loop/indexing | O(n) | O(n) for result array | Empty array, single element | Array concatenation can be done efficiently by pre-allocating array size. |
| 2 | [Number of Steps to Reduce a Number to Zero](https://leetcode.com/problems/number-of-steps-to-reduce-a-number-to-zero/) | Iterate while num > 0: if even divide by 2, if odd subtract 1, count steps | O(log n) | O(1) | n = 0, n = 1, large numbers | Bit manipulation alternative possible; iterative approach is clear. |
| 3 | [Contains Duplicate](https://leetcode.com/problems/contains-duplicate/) | Use HashSet to track seen elements, return true if duplicate found | O(n) | O(n) for HashSet | Empty array, all unique, all duplicates | HashSet provides O(1) lookup, ideal for duplicate detection. |
| 4 | [Best Time to Buy and Sell Stock](https://leetcode.com/problems/best-time-to-buy-and-sell-stock/) | Track minimum price seen so far, calculate max profit on each day | O(n) | O(1) | Decreasing prices (return 0), single day, all same | Single pass is sufficient; track min and max profit. |
| 5 | [Valid Anagram](https://leetcode.com/problems/valid-anagram/) | Character frequency count using array of size 26 or Dictionary | O(n) | O(1) (fixed alphabet size) or O(k) | Different lengths, same chars different order, empty strings | Frequency counting is efficient; can sort both strings as alternative O(n log n). |
