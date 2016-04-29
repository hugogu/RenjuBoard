#Renju Board#
## Introduction ##
As the name implies, *Renju Board* is a desktop application to play Renju. Like all other renju applications, it provides a basic AI to play against. But it is not only a game application for entertainment, its primary goal is to build up a platform so that people can write a Renju AI easily and allows AI vs AI tournament.

![](https://github.com/hugogu/RenjuBoard/blob/master/docs/images/MainWindow.png)

## What's new comparing with competitors ##

There already are a few such platforms out there, such as

- [Piskvork](https://sourceforge.net/projects/piskvork/)
- [Carbon-Gomoku](https://github.com/gomoku/Carbon-Gomoku)

But they are implemented 16 years ago in low level windows APIs which is not easy to extend and error-prone whereas this new *Renju Board* is written in C# with the most convenient windows UI framework. Providing a well-defined data structure and interfaces have been one of the key goals from the first day it was born, and the other key goals and features are:

- Debugging AI resolving steps visually.
- Providing a well-defined API and utilities to write a new AI easily.
- Support Piskvork protocol and native libs written under Piskvork.

## Roadmap ##

There are several key features to be implemented. The current plan is to finish the following items firstly:

- Stepping AI (:-))
- Sample AI (80%)
- Support All international rules (20%)
- Allow AI vs AI
- Detect Opening
- Debugging AI in VS
- Support piskvork
- A more sophisticated AI (Alpha beta pruning)


## References and Links##
[The International Rules of Renju](http://www.renju.net/study/rifrules.php)

[26 opening](http://www.renju.net/study/openings.php)

[Game Research and Technology](http://www.red3d.com/cwr/games/)

[AI Factory](http://www.aifactory.co.uk/)
