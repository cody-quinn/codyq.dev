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


@app.route('/api/auth/mc-plugins/{}')
def auth_mcplugins(uid):
    if uid == '65dc2c59-2e02-4e6b-9be1-e73c85ce0cdc':
        return {
            'uuid': '65dc2c59-2e02-4e6b-9be1-e73c85ce0cdc',
            'name': 'CustodyShop',
            'enabled': True,
            'paid': False
        }
    else:
        return {}