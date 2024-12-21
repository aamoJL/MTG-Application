# MTG-Application

Card collection, deck building and testing application for MTG card game

# Frameworks

* [C# WinUI 3](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/) (UI using MVVM architecture)
* [EntityFramework Core](https://learn.microsoft.com/en-us/ef/core/) (SQLite database)
* [LiveCharts2](https://lvcharts.com/docs/winui/2.0.0-beta.700/gallery) (Chart visuals)

# Deck Building

<img src="https://github.com/aamoJL/MTG-Application/assets/16759549/2529f7ea-f44f-499a-ba78-8536a9c3b0c1">

| Features  | Description |
| ------------- | ------------- |
| Scryfall search | Cards can be searched using the [Scryfall API](https://scryfall.com/docs/syntax). For example: `identity:gw type:human`. Search tab can be expanded by pressing the Search button. |
| Drag & Drop  | Cards can be added to lists by dragging and dropping. Pressing `Shift` while dragging will move the card from the origin list. |
| Import | Card lists can be imported from `text` or by `dragging and dropping EDHREC.com images`. |
| Export | Card lists can be exported using the card names or the Scryfall Id. |
| Charts | Charts of the deck statistics. |
| Card prints | Card's print can be selected by right clicking the card and selecting `Show prints...` |

<img src="https://github.com/aamoJL/MTG-Application/assets/16759549/6a77e2da-c3c2-41a8-972c-ffed6fcb3f2f" width=17%>

# Deck Testing

<img src="https://github.com/aamoJL/MTG-Application/assets/16759549/43e97a59-b98f-4493-8418-134b488cb78f" width=70%>

| Features | Description |
| ------------- | ------------- |
| Library | Cards can be added to the top or bottom of the library by dropping the cards to the library list. |
| Tapping | Cards can be tapped by `left clicking` the card. |
| Counters | **Plus counters** can be added to a card by `middle clicking` the card. **Count counters** can be added to a card by `right clicking` the card. Counter counts can be changed with the `scroll wheel`. Tokens will have the count counters visible by default. |
| Turns | Pressing the `New turn` button will automatically draw a card and untap all the cards on the battlefield. |

# Card Collections

<img src="https://github.com/aamoJL/MTG-Application/assets/16759549/7cba606a-b0e1-4e79-8165-76ff863e0f75" width=50%>

| Features | Description |
| ------------- | ------------- |
| Collection query | Collections can be created using the [Scryfall API](https://scryfall.com/docs/syntax) queries. |
| Owned cards | Card can be set as owned by `double clicking` the card or `left clicking` the card if the `Single click` toggle has been activated. |
