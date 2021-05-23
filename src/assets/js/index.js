// List of possible images to choose froom
const images = [
  "assets/img/Background0001.JPG",
  "assets/img/Background0002.JPG",
  "assets/img/Background0003.JPG",
  "assets/img/Background0004.JPG",
  "assets/img/Background0005.JPG",
  "assets/img/Background0006.JPG",
  "assets/img/Background0007.JPG",
];

// List of possible quotes to choose from
const quotes = [
  "I'm a masochist, also known as a programmer.",
  "I know JavaScript, Python, and Java!",
  "View my source on <a href='https://github.com/CatDevz/cody.me' target='_blank'>GitHub</a>!",
];

function setRandomImage() {
  // Getting the container DOM element, and a random image
  const containerDOM = document.getElementById("container");
  const randomImage = images[Math.floor(Math.random() * images.length)];
  const backgroundStyle = "url('" + randomImage + "')";

  // Applying the background style to the DOM element
  containerDOM.style.backgroundImage = backgroundStyle;
}

function setRandomQuote() {
  // Getting the quote DOM element and a random quote
  const quoteDOM = document.getElementById("quote");
  const quote = quotes[Math.floor(Math.random() * quotes.length)];

  // Applying the quote to the DOM element
  quoteDOM.innerHTML = quote;
}

function onLoad() {
  setRandomImage();
  setRandomQuote();
}

// Adding an event listener to call "onLoad" when the page loads
window.addEventListener("load", onLoad, false);
