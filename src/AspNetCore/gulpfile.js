/// <binding BeforeBuild='sass' ProjectOpened='default' />
'use strict';

var gulp = require( 'gulp' );
var sass = require( 'gulp-sass' );
var rename = require('gulp-rename');
var sourcemaps = require('gulp-sourcemaps');

var project = { webroot: './webroot'};//require( './project.json' );

var stylesrc = [project.webroot + '/sass/*.scss'];
var stylesrc_watch = [project.webroot + '/sass/**/*.scss'];

gulp.task('sass', function () {
    return gulp.src(stylesrc)
    .pipe(sourcemaps.init())
    .pipe(sass({ outputStyle: 'compressed' }).on('error', sass.logError))
    .pipe(rename({ extname: '.min.css' }))
    .pipe(sourcemaps.write('./'))
    .pipe(gulp.dest(project.webroot + '/css'));
});

gulp.task( 'watch', function () {
    gulp.watch( stylesrc_watch, ['sass'] );
} );

gulp.task( 'default', ['watch', 'sass'] );