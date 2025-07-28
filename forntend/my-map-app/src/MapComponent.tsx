import React, { useEffect, useRef, useImperativeHandle, forwardRef } from 'react';
import Map from 'ol/Map';
import View from 'ol/View';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import VectorLayer from 'ol/layer/Vector';
import VectorSource from 'ol/source/Vector';
import Draw, { DrawEvent } from 'ol/interaction/Draw';
import Modify, { ModifyEvent } from 'ol/interaction/Modify';
import Select from 'ol/interaction/Select';
import { click } from 'ol/events/condition';
import WKT from 'ol/format/WKT';
import { fromLonLat } from 'ol/proj';
import Feature, { FeatureLike } from 'ol/Feature';
import 'ol/ol.css';
import { Circle, Fill, Stroke, Style } from 'ol/style';

interface FeatureData {
    id: number;
    name: string;
    locationWkt: string;
    geometryType: string;
}

export interface MapComponentRef {
    clearLastDrawing: () => void;
}

interface MapComponentProps {
    onFeatureDrawn: (wkt: string) => void;
    featuresToDisplay: FeatureData[];
    editingFeature: FeatureData | null;
    onFeatureModified: (wkt: string) => void;
}

const styles = {
    'Point': new Style({ image: new Circle({ radius: 7, fill: new Fill({ color: 'rgba(231, 76, 60, 0.9)' }), stroke: new Stroke({ color: 'white', width: 2 }) }) }),
    'LineString': new Style({ stroke: new Stroke({ color: '#3498db', width: 3 }) }),
    'Polygon': new Style({ stroke: new Stroke({ color: '#2ecc71', width: 2 }), fill: new Fill({ color: 'rgba(46, 204, 113, 0.3)' }) }),
};
const styleFunction = (feature: FeatureLike) => {
    const geomType = feature.getGeometry()?.getType();
    return geomType ? styles[geomType as keyof typeof styles] : undefined;
}

const MapComponent = forwardRef<MapComponentRef, MapComponentProps>(
    ({ onFeatureDrawn, featuresToDisplay, editingFeature, onFeatureModified }, ref) => {
        const mapContainerRef = useRef<HTMLDivElement>(null);
        const mapRef = useRef<Map | null>(null);
        const vectorSourceRef = useRef(new VectorSource());
        const drawRef = useRef<Draw | null>(null);
        const selectRef = useRef<Select | null>(null);
        const modifyRef = useRef<Modify | null>(null);
        const lastFeatureRef = useRef<Feature | null>(null);

        useImperativeHandle(ref, () => ({
            clearLastDrawing() {
                if (lastFeatureRef.current && vectorSourceRef.current) {
                    vectorSourceRef.current.removeFeature(lastFeatureRef.current);
                    lastFeatureRef.current = null;
                }
            },
        }));

        useEffect(() => {
            if (!vectorSourceRef.current) return;
            const source = vectorSourceRef.current;
            source.clear();
            if (featuresToDisplay && featuresToDisplay.length > 0) {
                const wktFormat = new WKT();
                const olFeatures = featuresToDisplay.map(featureData => {
                    const feature = wktFormat.readFeature(featureData.locationWkt, {
                        dataProjection: 'EPSG:4326',
                        featureProjection: 'EPSG:3857',
                    });
                    feature.set('id', featureData.id);
                    return feature;
                });
                source.addFeatures(olFeatures);
            }
        }, [featuresToDisplay]);

        useEffect(() => {
            if (!mapRef.current || !vectorSourceRef.current) return;

            mapRef.current.getInteractions().forEach(interaction => {
                if (interaction instanceof Draw || interaction instanceof Select || interaction instanceof Modify) {
                    mapRef.current?.removeInteraction(interaction);
                }
            });

            if (editingFeature) {
                const selectInteraction = new Select({
                    style: new Style({
                        image: new Circle({ radius: 8, fill: new Fill({ color: '#FFD700' }) }),
                        stroke: new Stroke({ color: '#FFD700', width: 4 }),
                        fill: new Fill({ color: 'rgba(255, 215, 0, 0.3)' }),
                    }),
                });
                selectRef.current = selectInteraction;
                mapRef.current.addInteraction(selectInteraction);

                const modifyInteraction = new Modify({
                    features: selectInteraction.getFeatures(),
                });
                modifyRef.current = modifyInteraction;
                mapRef.current.addInteraction(modifyInteraction);

                modifyInteraction.on('modifyend', (event: ModifyEvent) => {
                    const modifiedFeature = event.features.getArray()[0];
                    const geometry = modifiedFeature.getGeometry();
                    if (geometry) {
                        const transformedGeom = geometry.clone().transform('EPSG:3857', 'EPSG:4326');
                        const wkt = new WKT().writeGeometry(transformedGeom);
                        onFeatureModified(wkt);
                    }
                });

                const featureToSelect = vectorSourceRef.current.getFeatureById(editingFeature.id);
                if (featureToSelect) {
                    selectInteraction.getFeatures().push(featureToSelect);
                }
            } else {
                addDrawInteraction('Point');
            }
        }, [editingFeature]);

        useEffect(() => {
            if (!mapContainerRef.current) return;

            const vectorLayer = new VectorLayer({ source: vectorSourceRef.current, style: styleFunction });
            const map = new Map({
                target: mapContainerRef.current,
                layers: [new TileLayer({ source: new OSM() }), vectorLayer],
                view: new View({ center: fromLonLat([35, 39]), zoom: 6 }),
            });
            mapRef.current = map;

            return () => map.setTarget(undefined);
        }, []);

        const addDrawInteraction = (drawType: 'Point' | 'Polygon' | 'LineString') => {
            if (!mapRef.current) return;

            if (drawRef.current) mapRef.current.removeInteraction(drawRef.current);
            if (selectRef.current) mapRef.current.removeInteraction(selectRef.current);
            if (modifyRef.current) mapRef.current.removeInteraction(modifyRef.current);

            const newDraw = new Draw({ source: vectorSourceRef.current, type: drawType });
            drawRef.current = newDraw;

            newDraw.on('drawend', (event: DrawEvent) => {
                lastFeatureRef.current = event.feature;
                const geometry = event.feature.getGeometry();
                if (geometry) {
                    const transformedGeom = geometry.clone().transform('EPSG:3857', 'EPSG:4326');
                    const wkt = new WKT().writeGeometry(transformedGeom);
                    onFeatureDrawn(wkt);
                }
            });
            mapRef.current.addInteraction(newDraw);
        };

        return (
            <div>
                <div ref={mapContainerRef} style={{ width: '100%', height: '500px', border: '1px solid #ccc' }}></div>
                <div className="draw-buttons" style={{ visibility: editingFeature ? 'hidden' : 'visible' }}>
                    <button onClick={() => addDrawInteraction('Point')}>Nokta Çiz</button>
                    <button onClick={() => addDrawInteraction('Polygon')}>Poligon Çiz</button>
                    <button onClick={() => addDrawInteraction('LineString')}>Çizgi Çiz</button>
                </div>
            </div>
        );
    });

export default MapComponent;