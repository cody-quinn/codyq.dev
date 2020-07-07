from flask import Flask, render_template, send_file, url_for, redirect
import random

app = Flask(__name__)


@app.route('/')
def home():
    quotes = \
        [
            'I develop software for the future.',
            'I develop software for the masses.',
            'Male, 15, living the cali dream.',
            'Check out <a href="https://github.com/CatDevz/COVIDWatchDiscordBot" class="quote" target="_blank">COVIDWatchDiscordBot<i class="fas fa-external-link-alt"></i></a>'
        ]
    return render_template('home.html', quote=random.choice(quotes))


@app.route('/api/picture')
def picture():
    return redirect(url_for('static', filename='img/bg-home/image{}.jpg'.format(random.randint(1,5))))