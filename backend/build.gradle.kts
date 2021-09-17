import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
    kotlin("jvm") version "1.4.32"
}

group = "me.codyq"
version = "1.0.0-SNAPSHOT"

repositories {
    mavenCentral()
}

dependencies {
    testImplementation(kotlin("test-junit"))

    implementation(platform("org.http4k:http4k-bom:4.12.0.0"))
    implementation("org.http4k:http4k-core:4.10.1.0")
    implementation("org.http4k:http4k-server-netty:4.10.1.0")
    implementation("org.http4k:http4k-client-apache:4.10.1.0")
    implementation("org.http4k:http4k-format-kotlinx-serialization:4.10.1.0")

    implementation("io.insert-koin:koin-core:3.1.2")
    testImplementation("io.insert-koin:koin-test:3.1.2")

    implementation("redis.clients:jedis:3.6.3")
}

tasks.test {
    useJUnit()
}

tasks.withType<KotlinCompile>() {
    kotlinOptions.jvmTarget = "11"
}